﻿namespace FCA.Core
{
    public static class Constants
    {
        public const string OtfRdsDataFcaWriter = "OtfRdsDataFca";
        public const string OtfRdsDataOtbaseWriter = "OtfRdsDataOtbase";
        public const string AwsAccessKey = "AwsAccessKey";
        public const string AwsSecretKey = "AwsSecretKey";
        public const string AwsRegion = "AwsRegion";
        public const string AwsDynamoPrefix = "AwsDynamo.Prefix";
        public const string CognitoClientIdKey = "CognitoClientId";
        public const string CognitoUserPoolIdKey = "CognitoUserPoolId";
        public const string CognitoAuthorityKey = "CognitoAuthority";

        //Websocket connection table
        public const string WEBSOCKET_TABLE = "WebConnection";
        public const string ConnectionIdField = "connectionId";
        public const string PublicationId = "publicationId";

        public static class Division
        {
            public const string State = "state";
            public const string Country = "country";
        }
    }
}