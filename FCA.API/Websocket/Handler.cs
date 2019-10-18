using System;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Amazon.Lambda.DynamoDBEvents;

using FCAWS.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace FCAWS
{
    public class Handler
    {
        IAmazonDynamoDB _ddbClient = new AmazonDynamoDBClient();
        public async Task<APIGatewayProxyResponse> OnConnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Using JObject instead of APIGatewayProxyRequest till APIGatewayProxyRequest gets updated with DomainName and ConnectionId 
                var connectionId = request.RequestContext.ConnectionId;
                context.Logger.LogLine($"ConnectionId: {connectionId}");

                var body = JObject.FromObject(request).ToString();
                var ddbRequest = new PutItemRequest
                {
                    TableName = Constants.TABLE_NAME,
                    Item = new Dictionary<string, AttributeValue>
                    {
                        {Constants.ConnectionIdField, new AttributeValue{ S = connectionId}},
                        {Constants.RequestBodyField, new AttributeValue{ S = body}}
                    }
                };

                await _ddbClient.PutItemAsync(ddbRequest);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error connecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to connect: {e.Message}"
                };
            }
        }
        public async Task<APIGatewayProxyResponse> OnDisconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Using JObject instead of APIGatewayProxyRequest till APIGatewayProxyRequest gets updated with DomainName and ConnectionId 
                var connectionId = request.RequestContext.ConnectionId;
                context.Logger.LogLine($"ConnectionId: {connectionId}");
                var body = JObject.FromObject(request).ToString();
                var ddbRequest = new DeleteItemRequest
                {
                    TableName = Constants.TABLE_NAME,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        {Constants.ConnectionIdField, new AttributeValue {S = connectionId}}
                    }
                };

                await _ddbClient.DeleteItemAsync(ddbRequest);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Disconnected."
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to disconnect: {e.Message}"
                };
            }
        }
        public async Task<APIGatewayProxyResponse> OnSendMessage(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                // Using JObject instead of APIGatewayProxyRequest till APIGatewayProxyRequest gets updated with DomainName and ConnectionId 
                var domainName = request.RequestContext.DomainName;
                var stage = request.RequestContext.Stage;
                var endpoint = $"https://{domainName}/{stage}";
                context.Logger.LogLine($"API Gateway management endpoint: {endpoint}");

                var message = JsonConvert.DeserializeObject<JObject>(request.Body);
                var data = message["data"]?.ToString();
                context.Logger.LogLine($"API Gateway message: {message}");
                var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(data));

                var scanRequest = new ScanRequest
                {
                    TableName = Constants.TABLE_NAME,
                    ProjectionExpression = Constants.ConnectionIdField
                };

                var scanResponse = await _ddbClient.ScanAsync(scanRequest);

                var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
                {
                    ServiceURL = endpoint
                });

                return await _broadcast(scanResponse, apiClient, stream, context);
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to send message: {e.Message}"
                };
            }

        }

        private async Task<APIGatewayProxyResponse> _broadcast(ScanResponse list, AmazonApiGatewayManagementApiClient apiClient, MemoryStream stream, ILambdaContext context)
        {
            var count = 0;
            foreach (var item in list.Items)
            {
                var connectionId = item[Constants.ConnectionIdField].S;


                var postConnectionRequest = new PostToConnectionRequest
                {
                    ConnectionId = connectionId,
                    Data = stream
                };

                try
                {
                    LambdaLogger.Log($"Post to connection {count}: {connectionId}");
                    stream.Position = 0;
                    await apiClient.PostToConnectionAsync(postConnectionRequest);
                    count++;
                }
                catch (AmazonServiceException e)
                {
                    // API Gateway returns a status of 410 GONE when the connection is no
                    // longer available. If this happens, we simply delete the identifier
                    // from our DynamoDB table.
                    if (e.StatusCode == HttpStatusCode.Gone)
                    {
                        var ddbDeleteRequest = new DeleteItemRequest
                        {
                            TableName = Constants.TABLE_NAME,
                            Key = new Dictionary<string, AttributeValue>
                                {
                                    {Constants.ConnectionIdField, new AttributeValue {S = connectionId}}
                                }
                        };

                        context.Logger.LogLine($"Deleting gone connection: {connectionId}");
                        await _ddbClient.DeleteItemAsync(ddbDeleteRequest);
                    }
                    else
                    {
                        context.Logger.LogLine($"Error posting message to {connectionId}: {e.Message}");
                        context.Logger.LogLine(e.StackTrace);
                    }
                }
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Data send to " + count + " connection" + (count == 1 ? "" : "s")
            };
        }

        public async Task<APIGatewayProxyResponse> UpdateVersion(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"request: {request.Body}");
                var data = JsonConvert.DeserializeObject<FCAVersion>(request.Body);
                context.Logger.LogLine($"API Gateway message: {request.Body}");
                var ddbRequest = new PutItemRequest
                {
                    TableName = Constants.APP_VERSION_TABLE,
                    Item = new Dictionary<string, AttributeValue>
                    {
                        {Constants.Id, new AttributeValue{ S =data.id}},
                        {Constants.ApplicationId, new AttributeValue{ S = data.applicationId}},
                        {Constants.Version, new AttributeValue{ S = data.version}}
                    }
                };

                await _ddbClient.PutItemAsync(ddbRequest);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Update version successfully."
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to send message: {e.Message}"
                };
            }

        }
        public async Task<APIGatewayProxyResponse> StreamReceiver(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            var scanRequest = new ScanRequest
            {
                TableName = Constants.TABLE_NAME,
                ProjectionExpression = Constants.ConnectionIdField
            };

            var scanResponse = await _ddbClient.ScanAsync(scanRequest);

            var appVersion = _getApplicationVersion(dynamoEvent, context);
            var appVersionS = JObject.FromObject(appVersion).ToString();
            context.Logger.LogLine("App version: " + appVersionS);
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(appVersionS));

            var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
            {
                ServiceURL = "https://rpi4kpxg33.execute-api.us-east-1.amazonaws.com/Prod"
            });
            return await _broadcast(scanResponse, apiClient, stream, context);
        }
        private FCAVersion _getApplicationVersion(DynamoDBEvent dEvent, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine("DynamoDBEvent " + JObject.FromObject(dEvent).ToString());

                var record = dEvent.Records[0];
                var element = record.Dynamodb.NewImage;

                var result = new FCAVersion
                {
                    id = element[Constants.Id].S,
                    applicationId = element[Constants.ApplicationId].S,
                    version = element[Constants.Version].S
                };

                return result;
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error when get Application version: " + e.Message);
                return null;
            }
        }

    }
}
