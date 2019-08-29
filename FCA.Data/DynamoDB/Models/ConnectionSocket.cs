using System;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace FCA.Data.DynamoDB.Models
{
    [DynamoDBTable("ConnectionSocket")]
    public class ConnectionSocket
    {
        [DynamoDBHashKey("channel_id")]
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        
        [DynamoDBProperty("message_id")]
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
        
        [DynamoDBProperty("ConnectionId")]
        [JsonProperty("connectionId")]
        public string ConnectionId { get; set; }

        [DynamoDBProperty("Content")]
        [JsonProperty("content")]
        public string Content { get; set; }
        
        [DynamoDBProperty("Name")]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}