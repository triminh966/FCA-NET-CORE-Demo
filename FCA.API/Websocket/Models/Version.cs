using Amazon.DynamoDBv2.DataModel;

namespace FCAWS.Models
{
    [DynamoDBTable(Constants.APP_VERSION_TABLE)]
    public class FCAVersion
    {
        [DynamoDBHashKey]
        public string id { get; set; }

        public string applicationId { get; set; }

        public string version { get; set; }
    }
}