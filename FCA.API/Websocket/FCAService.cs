using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FCAServices
{
    public class FCA
    {
        public APIGatewayProxyResponse Authentication(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var body = JObject.FromObject(request).ToString();
                context.Logger.LogLine($"request : {body}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Authentication"
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to authenticate: {e.Message}"
                };
            }
        }
        public APIGatewayProxyResponse AuthorPublish(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"request : {JObject.FromObject(request).ToString()}");
                var publicationId = request.Headers["PublicationId"];
                context.Logger.LogLine($"publicationId : {publicationId}");
                string queryString = $"SELECT * FROM  PubSub_Dev.Publication WHERE PubId = '{publicationId}'";
                var isHasPublicationId = false;
                using (MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("Database")))
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = queryString;
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    context.Logger.LogLine($"reader : {reader.HasRows}");
                    if (reader.HasRows)
                    {
                        isHasPublicationId = true;
                    }
                }
                if (isHasPublicationId)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var urlParameters = "/Dev/publication";
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

                        //Get data
                        var data = new StringContent(request.Body);

                        // Data response.
                        HttpResponseMessage response = client.PostAsync(urlParameters, data).Result;
                        context.Logger.LogLine("Response: " + JObject.FromObject(response).ToString());
                    }
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Body = "Authorzied"
                    };
                }
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = "Unauthorzied"
                };
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Error disconnecting: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to authenticate: {e.Message}"
                };
            }

        }
     
    }
}
