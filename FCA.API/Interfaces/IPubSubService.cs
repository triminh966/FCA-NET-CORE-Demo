using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace FCA.API.Interfaces
{
    public interface IPubSubService
    {
        APIGatewayCustomAuthorizerResponse CustomAuthor(APIGatewayCustomAuthorizerRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> Publish(APIGatewayProxyRequest request, ILambdaContext context);
    }
}
