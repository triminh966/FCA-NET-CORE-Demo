using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FCA.Core;
using Newtonsoft.Json.Linq;

namespace FCA.API.Services
{
    public class PubSubService : IPubSubService
    {
        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

        IAmazonDynamoDB _ddbClient = new AmazonDynamoDBClient();

        public APIGatewayCustomAuthorizerResponse CustomAuthor(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"request: {JObject.FromObject(request).ToString()}");
                var token = request.QueryStringParameters["Authorization"];
                var appName = request.Headers["AppName"];
                var publicationId = request.Headers["PublicationId"];
                if (!String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(appName) && !String.IsNullOrEmpty(publicationId))
                {
                    //Get private URL service from Parameter Store
                    var serviceUrl = GetParameterStore(appName);
                    context.Logger.LogLine($"private URL: {serviceUrl}");

                    var isAuthenticated = ConnectPrivateApi(serviceUrl, token, publicationId, context);
                    context.Logger.LogLine($"isAuthenticated: {isAuthenticated}");
                    if (isAuthenticated.IsSuccessStatusCode)
                    {
                        return GeneratePolicy("me", "Allow", request.MethodArn);
                    }
                    return GeneratePolicy("me", "Deny", request.MethodArn);
                }
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"[WS] Error authorizer: {e.Message}. Exception: {e}");
            }
            return GeneratePolicy("me", "Deny", request.MethodArn);
        }
        public async Task<APIGatewayProxyResponse> OnConnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {

                var payload = JObject.FromObject(request).ToString();
                context.Logger.LogLine($"payload: {payload}");

                var connectionId = request.RequestContext.ConnectionId;
                var publicationId = request.Headers["PublicationId"];

                //Save connectionId to dynamoDB
                var ddbRequest = new PutItemRequest
                {
                    TableName = Constants.WEBSOCKET_TABLE,
                    Item = new Dictionary<string, AttributeValue>
                    {
                        {Constants.ConnectionIdField, new AttributeValue{ S = connectionId}},
                        {Constants.PublicationId, new AttributeValue{ S = publicationId}}
                    }
                };
                await _ddbClient.PutItemAsync(ddbRequest);

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

        public async Task<APIGatewayProxyResponse> OnDisconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
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

        public async Task<APIGatewayProxyResponse> PublishMessage(APIGatewayProxyRequest request, ILambdaContext context)
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
        public HttpResponseMessage ConnectPrivateApi(string privateURL, string token, string publicationId, ILambdaContext context)
        {
            using (HttpClient client = new HttpClient())
            {
                context.Logger.LogLine("DNS: " + Environment.GetEnvironmentVariable("DNSName"));
                context.Logger.LogLine("APIGW: " + Environment.GetEnvironmentVariable("APIGW"));
                var builder = new UriBuilder()
                {
                    Host = Environment.GetEnvironmentVariable("DNSName"),
                    Port = 443,
                    Scheme = Uri.UriSchemeHttps,
                };
                context.Logger.LogLine("Uri: " + builder.Uri.ToString());
                client.BaseAddress = new Uri(builder.Uri.ToString());
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Host = Environment.GetEnvironmentVariable("APIGW");
                client.DefaultRequestHeaders.Add("PublicationId", publicationId);
                client.DefaultRequestHeaders.Add("Authorization", token);

                // Data response.
                HttpResponseMessage response = client.PostAsync(privateURL, null).Result;
                context.Logger.LogLine("Response: " + JObject.FromObject(response).ToString());
                return response;
            }

        }
        private string GetParameterStore(string appName)
        {
            var client = new AmazonSimpleSystemsManagementClient(_region);
            var requestParam = new GetParametersRequest
            {
                Names = new List<string> { appName }
            };
            var response = client.GetParametersAsync(requestParam).Result;
            var value = response.Parameters.Single().Value;
            var data = JObject.Parse(value)["authenticateUrl"]?.ToString();
            return data;
        }
        private APIGatewayCustomAuthorizerResponse GeneratePolicy(string principalId, string effect, string resouce)
        {
            var statement = new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement();
            if (!String.IsNullOrEmpty(effect) && !String.IsNullOrEmpty(resouce))
            {
                statement.Action = new HashSet<string>(new string[] { "execute-api:Invoke" });
                statement.Effect = effect;
                statement.Resource = new HashSet<string>(new string[] { resouce });
            }

            var authPolicy = new APIGatewayCustomAuthorizerResponse();
            authPolicy.PrincipalID = principalId;
            authPolicy.PolicyDocument = new APIGatewayCustomAuthorizerPolicy();
            authPolicy.PolicyDocument.Version = "2012-10-17";

            authPolicy.PolicyDocument.Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>();
            authPolicy.PolicyDocument.Statement.Add(statement);

            return authPolicy;
        }
    }
}
