namespace FCA.Core.Secrets
{
    public interface ICognitoSecrets
    {
        string ClientId { get; }
        string UserPoolId { get; }
        string AuthorityUrl { get; }
    }
}
