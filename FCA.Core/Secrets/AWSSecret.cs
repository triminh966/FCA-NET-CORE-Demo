using Amazon;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace FCA.Core.Secrets
{
    public interface IAwsSecret
    {
        string AwsAccessKey { get; }
        string AwsSecretKey { get; }
        RegionEndpoint AwsRegion { get; }
        AWSCredentials AwsCredentials { get; }
        string AwsDynamoPrefix { get; }
        DynamoDBOperationConfig DefaultDbOperationConfig { get; }
    }
}
