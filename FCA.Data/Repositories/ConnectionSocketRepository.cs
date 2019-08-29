using FCA.Core.Secrets;
using FCA.Data.DynamoDB.Models;
using FCA.Data.DynamoDB.Repositories;

namespace FCA.Data.Repositories
{
    public interface IConnectionSocketRepository : IGenericAWSRepository<ConnectionSocket>
    {
    }
    public class ConnectionSocketRepository : GenericAWSRepository<ConnectionSocket>, IConnectionSocketRepository
    {
        public ConnectionSocketRepository(FcaSecrets fcaSecrets) : base(fcaSecrets)
        {
        }
    }
}