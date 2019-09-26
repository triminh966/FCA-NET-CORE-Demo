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
                _issuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_JjenpQv4w";
                _audience = "qqvfrtr9fs1k2mtbbcl0nv7um";
                _tokenUse = "id";

                var kid1 = "RszJ4abPHdTvPeUYfeJgQMRQFmoV28T4A85d1l/Qe08=";
                var e1 = "AQAB";
                var n1 = "lST0mtdNSRBr09mDQnrXDMIgCJ6WIHalE1A-4ePraNqMabn_RukZGGbw-kGzdqi6njDp0P4CCRW6H8qGRYikrMt1ezAiX_KfG1uVONhoIsLGHTybRIKj_x1X__V27FGQy2NFh3JsPiKYCPaNDU0afsPqSdLopMnc_bzBdPZAfOnbR7PliyHJ-lolwUyM6AqSyGioUThYkFuqa_ywI0W0zAyNC2KfyBryN0jgXVp5M-82N86YpIGx9ecVi79l_5y2nnKNIJHf_Oj_uZFtLV4HstlQ5lizbbvxtTXDca8SIa9pBx_ff20xj6SKHJXa2CHG8UaBva4eyS_2i2gJTwxNXQ";
                if (!string.IsNullOrEmpty(kid1))
                {
                    _keys.Add(new RsaSecurityKey(new RSAParameters { Exponent = Base64UrlEncoder.DecodeBytes(e1), Modulus = Base64UrlEncoder.DecodeBytes(n1) })
                    {
                        KeyId = kid1
                    });
                }

                var kid2 = "HDr4mXIRGSS8YvGWBRF8dqkGh5akV3gl3u1NYMJpcH4=";
                var e2 = "AQAB";
                var n2 = "szZFPShF1HE3GaU_ZPAQyniVYwYOXblv1W4HwHmi2hB-MS8LsFxwY_i50CXVJTg_oZ_tfkPns6wjfVy-IiHE2f6cnlBq8pgSYZcXOt_NnC70caERBxy5uSl9mNxCAqnIDmHx86b5j0vJOpwS1UvbYRRBREXd-u4akCnsIJm0P4b9Vc2Nx_xD_AKT-6FO9PmtBv8wCULOQmPkbev395FpbO1q2KgxGEIJdFu5Jh1w1aI965GgwjEU-M-H6152vDTZIJee9I21Ah4GAn-d8gveflcV4TDmsTFosmbcKDoxA0Qf-IMY24VyJ7ZuGXjfkyLum60v4EngD3pFyo-770mpPQ";
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
