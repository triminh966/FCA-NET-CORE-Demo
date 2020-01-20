using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace FCA.API.Interfaces
{
    public interface IPubSubService
    {
        APIGatewayCustomAuthorizerResponse CustomAuthor(APIGatewayCustomAuthorizerRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> OnConnect(APIGatewayProxyRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> OnDisconnect(APIGatewayProxyRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> Publish(APIGatewayProxyRequest request, ILambdaContext context);
    }
}
