using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using FCA.API.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace FCA.API.Lambda.Websocket
{
    public class PubSubFunction
    {
        private readonly IPubSubService _pubSubService = new PubSubService();

        public APIGatewayCustomAuthorizerResponse CustomAuthor(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            var result = _pubSubService.CustomAuthor(request, context);
            return  result;
        }

        public async Task<APIGatewayProxyResponse> OnConnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = _pubSubService.OnConnect(request, context);
            return await result ;
        }

        public async Task<APIGatewayProxyResponse> OnDisconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = _pubSubService.OnDisconnect(request, context);
            return await result;
        }

        public async Task<APIGatewayProxyResponse> Publication(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var result = _pubSubService.PublishMessage(request, context);
            return await result;
        }
    }
}


