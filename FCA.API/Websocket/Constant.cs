using System;
namespace FCAWS
{
    public class Constants
    {
        //Websocket connection
        public static readonly string WEBSOCKET_TABLE = "WebConnection";
        public const string ConnectionIdField = "connectionId";
        public const string RequestBodyField = "requestBody";
        public const string PublicationId = "publicationId";
        //Application vestion
        public static readonly string APP_VERSION_TABLE = "FCAVersion";
        public const string Id = "id";
        public const string ApplicationId = "applicationId";
        public const string Version = "version";
    }
}
