using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace FCA.API.Services.Websocket
{
    public class AWSSystemManagerService
    {
        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;
        public string GetParameterStore(string appName,string key)
        {
            var client = new AmazonSimpleSystemsManagementClient(_region);
            var requestParam = new GetParametersRequest
            {
                Names = new List<string> { appName }
            };
            var response = client.GetParametersAsync(requestParam).Result;
            var value = response.Parameters.Single().Value;
            var data = JObject.Parse(value)[key]?.ToString();
            return data;
        }
    }
}
