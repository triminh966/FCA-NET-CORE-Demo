using Amazon;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FCAWS
{
    public class CustomAuthor
    {
        public APIGatewayCustomAuthorizerResponse Handler(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"request: {JObject.FromObject(request).ToString()}");

            try
            {
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

                //Get data
                var data = new StringContent("aaa");

                // Data response.
                HttpResponseMessage response = client.PostAsync(privateURL,data).Result;
                context.Logger.LogLine("Response: " + JObject.FromObject(response).ToString());
                return response;
            }

        }
        private string GetParameterStore(string appName)
        {
            var client = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1);
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
