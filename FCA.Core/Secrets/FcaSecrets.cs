using Amazon;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FCA.Core.Secrets
{
    public interface IFcaSecrets : IConnectionStringSecret, ICognitoSecrets, IAwsSecret
    {
    }

    public class FcaSecrets : IFcaSecrets
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ISecretManager _secretManager;

        public FcaSecrets(IHostingEnvironment environment, IConfiguration configuration)
        {
            _secretManager = new AbstractSecretManager(configuration);
            _environment = environment;
            _configuration = configuration;

            Initialise();
        }

        private void Initialise()
        {
            //if (_environment.IsDevelopment())
            //{
                ReadLocalSecret();
            //}
            //else
            //{
            //    OtfRdsDataFca = _secretManager.RetrieveSecretValue(Constants.OtfRdsDataFcaWriter);
            //    OtfRdsDataOTbase = _secretManager.RetrieveSecretValue(Constants.OtfRdsDataOtbaseWriter);
            //    AwsAccessKey = _secretManager.RetrieveSecretValue(Constants.AwsAccessKey);
            //    AwsSecretKey = _secretManager.RetrieveSecretValue(Constants.AwsSecretKey);
            //    AwsRegion = RegionEndpoint.GetBySystemName(_secretManager.RetrieveSecretValue(Constants.AwsRegion));
            //    AwsCredentials = new BasicAWSCredentials(AwsAccessKey, AwsSecretKey);
            //    AwsDynamoPrefix = _secretManager.RetrieveSecretValue(Constants.AwsDynamoPrefix);
            //    DefaultDbOperationConfig = new DynamoDBOperationConfig { SkipVersionCheck = false, ConsistentRead = true, TableNamePrefix = AwsDynamoPrefix };
            //    ClientId = _secretManager.RetrieveSecretValue(Constants.CognitoClientIdKey);
            //    AuthorityUrl = _secretManager.RetrieveSecretValue(Constants.CognitoAuthorityKey);
            //    UserPoolId = _secretManager.RetrieveSecretValue(Constants.CognitoUserPoolIdKey);
            //}
        }

        private void ReadLocalSecret()
        {
            OtfRdsDataFca = _configuration["connectionStrings:OtfRdsDataFca"];
            OtfRdsDataOTbase = _configuration["connectionStrings:OtfRdsDataOTbase"];

            AwsAccessKey = _configuration["AWS:AccessKey"];
            AwsSecretKey = _configuration["AWS:SecretKey"];
            AwsRegion = RegionEndpoint.GetBySystemName(_configuration["AWS:Region"]);
            AwsDynamoPrefix = _configuration["AWS:Dynamo.Prefix"];

            ClientId = _configuration["Cognito:ClientId"];
            UserPoolId = _configuration["Cognito:UserPoolId"];
            AuthorityUrl = _configuration["Cognito:AuthorityUrl"];

            AwsCredentials = new BasicAWSCredentials(AwsAccessKey, AwsSecretKey);
            DefaultDbOperationConfig = new DynamoDBOperationConfig { SkipVersionCheck = false, ConsistentRead = true, TableNamePrefix = AwsDynamoPrefix };
        }
        
        public string OtfRdsDataFca { get; private set; }
        public string OtfRdsDataOTbase { get; private set; }
        public string AwsAccessKey { get; private set; }
        public string AwsSecretKey { get; private set; }
        public RegionEndpoint AwsRegion { get; private set; }
        public AWSCredentials AwsCredentials { get; private set; }
        public string AwsDynamoPrefix { get; private set; }
        public DynamoDBOperationConfig DefaultDbOperationConfig { get; private set; }
        public string ClientId { get; private set; }
        public string UserPoolId { get; private set; }
        public string AuthorityUrl { get; private set; }
    }
}