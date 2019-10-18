using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;

namespace FCAWS
{
    public class Author
    {
        private readonly List<SecurityKey> _keys = new List<SecurityKey>();
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _tokenUse;

        public Author()
        {
            try
            {
                //using sit fca
                //us-east-1_qswGT8ImC match your user pool id
                _issuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_QPPfSKreN";
                //app client id
                _audience = "pt57be8gffvg40nuhntee1k3a";
                //If you are only accepting the access token in your web APIs, its value must be access.
                //If you are only using the ID token, its value must be id.
                //If you are using both ID and access tokens, the token_use claim must be either id or access
                _tokenUse = "id";

                //Json web key: https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/jwks.json.
                var kid1 = "+o+Bniqg5wsp8Xyk1B1CVW7W2nCpWWna/CK6ZAAK0eU=";
                var e1 = "AQAB";
                var n1 = "hVKyZCeAZAeAdOeLnz5tFJH91mZk_kypEt0KzTp9PXP-GdqDxiKh4YvMWMNyyzuhzhGcpQxvG3Vg8PF6ReCyWYM2ROJYKElNaEiocuTsK99QTC37TJR-U5HQPT4tolJts7tOAf8sFa0AwpD9WiepOtAhk2mj3OMa-WEa3YxpK2ucmoPHWNyeZxjFskqq72Zvyuf8BcXmPjsv4agNyi9Yl0FV8EqQyzWVrAasFBpdpzkJre98jkXbofyfPB9fRLmr65e9CAWCUdWQ0H4bZc5DC2-exF09laWnCRx_UNd3ZE_5T2q9OSsb7tc1k33jnMw59Zi4nKKGyMDSSSixT1HonQ";
                if (!string.IsNullOrEmpty(kid1))
                {
                    _keys.Add(new RsaSecurityKey(new RSAParameters { Exponent = Base64UrlEncoder.DecodeBytes(e1), Modulus = Base64UrlEncoder.DecodeBytes(n1) })
                    {
                        KeyId = kid1
                    });
                }

                var kid2 = "mwpBgLy7t+he432sb5TQGYL8MXwuYM1aYT6IYQuNrKg=";
                var e2 = "AQAB";
                var n2 = "o_pfAb51h2uYGSkyjgArQX7dyECCoXOQBjaRmXoe4pM-F3Tg7Mf5QOImqusLqMMzrz8JPZsGN7WOaaGgbGplboA0VL86L1d7T5cllRZFAq9FArIq0ED_1l8IxGGWELvSaTfflYL-GyRfgxExzgXepBZP7CIAlBYyzavxMt_O6PXPFPrqs9aX6Mr-t6IQgBqOGfRlAVN9BU8z-6Wh-w_gW1D7aYkvgMWle2E_Vu5yE6bdxEUTVVkMXQ8dP4i-7W2Tyki8D1xXOCGSA3sRA5a5bS07Uz6sNJ_6yEmTzhrvfmWXjd58DBZhyMY0uiJlAbrkJBLpdLIV3IDa0CIbBx5yWQ";
                if (!string.IsNullOrEmpty(kid2))
                {
                    _keys.Add(new RsaSecurityKey(new RSAParameters { Exponent = Base64UrlEncoder.DecodeBytes(e2), Modulus = Base64UrlEncoder.DecodeBytes(n2) })
                    {
                        KeyId = kid2
                    });
                }
            }
            catch (Exception e)
            {
                LambdaLogger.Log($"[WS] Error authorizer: {e.Message}. Exception: {e}");
            }
        }
        public APIGatewayCustomAuthorizerResponse FunctionHandler(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            var key = request.QueryStringParameters["AuthKey"];
            var methodArn = request.MethodArn;


            context.Logger.LogLine($"Request: {JObject.FromObject(request).ToString()}");
            context.Logger.LogLine($"Context: {JObject.FromObject(context).ToString()}");
            context.Logger.LogLine($"Key: {key}");
            context.Logger.LogLine($"methodArn: {methodArn}");

            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    var tokenToValidate = key;
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeys = _keys,
                        ValidIssuer = _issuer,
                        ValidAudience = _audience,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(0)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var result = tokenHandler.ValidateToken(tokenToValidate, tokenValidationParameters, out _);
                    var iss = result.Claims.FirstOrDefault(x => x.Type == "iss")?.Value;
                    var aud = result.Claims.FirstOrDefault(x => x.Type == "aud")?.Value;
                    var tokenUse = result.Claims.FirstOrDefault(x => x.Type == "token_use")?.Value;
                    if (string.Equals(iss, _issuer, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(aud, _audience, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(tokenUse, _tokenUse, StringComparison.OrdinalIgnoreCase))
                    {
                        return GeneratePolicy("me", "Allow", methodArn);
                    }
                }
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"[WS] Error authorizer: {e.Message}. Exception: {e}");
            }
            return GeneratePolicy("me", "Deny", methodArn);
        }
        private APIGatewayCustomAuthorizerResponse GeneratePolicy(string principalId, string effect, string resouce)
        {
            var statement = new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement();
            if (!String.IsNullOrEmpty(effect) && !String.IsNullOrEmpty(resouce))
            {
                statement.Action = new HashSet<string>(new string[] { "execute-api:Invoke" });
                statement.Effect = effect;
                statement.Resource = new HashSet<string>(new string[] { resouce });
            }

            var authPolicy = new APIGatewayCustomAuthorizerResponse();
            authPolicy.PrincipalID = principalId;
            authPolicy.PolicyDocument = new APIGatewayCustomAuthorizerPolicy();
            authPolicy.PolicyDocument.Version = "2012-10-17";

            authPolicy.PolicyDocument.Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>();
            authPolicy.PolicyDocument.Statement.Add(statement);

            return authPolicy;
        }
    }
}
