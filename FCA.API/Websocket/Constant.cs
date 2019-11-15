using System;
namespace FCAWS
{
    public class Constants
    {
        //Websocket connection
        public const string WEBSOCKET_TABLE = "WebConnection";
        public const string ConnectionIdField = "connectionId";
        public const string RequestField = "payload";
        public const string PublicationId = "publicationId";

        //Application vesion
        public const string APP_VERSION_TABLE = "AppVersion";
        public const string Id = "id";
        public const string ApplicationId = "applicationId";
        public const string Version = "version";

        public const string PulicationFCA = "c0a5bf76-bd5f-4f0e-9a91-68e183b4f01c";
        public const string PrivateURL = "https://p68tyum404.execute-api.us-east-1.amazonaws.com/Prod/authen";
    }
}
