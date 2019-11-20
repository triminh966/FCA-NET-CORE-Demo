namespace FCA.Core.Secrets
{
    public interface IConnectionStringSecret
    {
        string OtfRdsDataFca { get; }
        string OtfRdsDataOTbase { get; }
        string OtfRdsDataPubSub { get; }
    }
}