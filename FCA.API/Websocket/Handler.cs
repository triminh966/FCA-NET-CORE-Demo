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

using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Net.Http.Headers;


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

                var payload = JObject.FromObject(request).ToString();
                context.Logger.LogLine($"payload: {payload}");

                var connectionId = request.RequestContext.ConnectionId;
                var publicationId = request.Headers["PublicationId"];

                //Save connectionId to dynamoDB
                await SaveConnection(connectionId, publicationId);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Connected"
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
        private async Task<PutItemResponse> SaveConnection(string connectionId, string publicationId)
        {
            var ddbRequest = new PutItemRequest
            {
                TableName = Constants.WEBSOCKET_TABLE,
                Item = new Dictionary<string, AttributeValue>
                    {
                        {Constants.ConnectionIdField, new AttributeValue{ S = connectionId}},
                        {Constants.PublicationId, new AttributeValue{ S = publicationId}}
                    }
            };

            return await _ddbClient.PutItemAsync(ddbRequest);
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
                    TableName = Constants.WEBSOCKET_TABLE,
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
                            TableName = Constants.WEBSOCKET_TABLE,
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
        public async Task<APIGatewayProxyResponse> Publication(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"request : {JObject.FromObject(request).ToString()}");

                var publicationId = request.Headers["PublicationId"];
                context.Logger.LogLine($"publicationId : {publicationId}");

                var scanRequest = new ScanRequest
                {
                    TableName = Constants.WEBSOCKET_TABLE,
                    ReturnConsumedCapacity = "TOTAL",
                    ProjectionExpression = "connectionId,#pubId",
                    ExpressionAttributeNames = new Dictionary<string, string> { { "#pubId", "publicationId" } },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":v_publicationId", new AttributeValue { S = publicationId } } },
                    FilterExpression = "#pubId = :v_publicationId",
                };

                var scanResponse = await _ddbClient.ScanAsync(scanRequest);
                var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
                {
                    ServiceURL = Environment.GetEnvironmentVariable("PubSubWSGateway")
                });
                var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(request.Body));

                return await _broadcast(scanResponse, apiClient, stream, context);

            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to publication: {e.Message}"
                };
            }

        }
    }
}

