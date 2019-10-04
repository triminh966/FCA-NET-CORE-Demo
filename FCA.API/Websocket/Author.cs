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
                //us-east-1_qswGT8ImC match your user pool id
                _issuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_qswGT8ImC";
                //app client id
                _audience = "34ijboia9jc7k4rqbcuu7klo10";
                //If you are only accepting the access token in your web APIs, its value must be access.
                //If you are only using the ID token, its value must be id.
                //If you are using both ID and access tokens, the token_use claim must be either id or access
                _tokenUse = "id";

                //Json web key: https://cognito-idp.{region}.amazonaws.com/{userPoolId}/.well-known/jwks.json.
                var kid1 = "QjeLVmm7EgFtEoJwpNp4kvVGLqI124dXi4uG371TyJ0=";
                var e1 = "AQAB";
                var n1 = "qVVICGrJqqSaXbjF5ZHxZoHBgbDMlFgq7vQfUZXyoFM8GFnG_1q59jbRPtoqWTwWNNhAqVGv9AhxjnHUJ2Vqks0lSLdO1jlEJ4ECJOf5wb9c2Vo8U524l-rs2AXT0kA7G_uG_QweR_8E7Enr2mZjCaxVK0MO-KQspSLjkuFEUpCoVpjrSeYXQn-PD_niCTuD3aSDKQO-nKSVifYn56XuJGogfp4Mp8f2o0Qm0NNketRgmG9rIZtOIKV5AOatBCxYFQ57E2v-AhetBeR4t3fFclutXTQNeFLD2dy_UI9CWgBajQw-QX12ciu8aM0JL3MW8tQa3yd1G-ASNYLXl_C5rQ";
                if (!string.IsNullOrEmpty(kid1))
                {
                    _keys.Add(new RsaSecurityKey(new RSAParameters { Exponent = Base64UrlEncoder.DecodeBytes(e1), Modulus = Base64UrlEncoder.DecodeBytes(n1) })
                    {
                        KeyId = kid1
                    });
                }

                var kid2 = "vyHa7qUxNdIZBNgMsG+QBwYl4YigwMC/N/+Fgj2flv8=";
                var e2 = "AQAB";
                var n2 = "gvNcubvQOhTLPSjTfX00wY1jjyigV0_myzQjTaol7kCCti9jaWoNVuxtJ3pBB21-fY0d0OD-4Yru5uZ2b4wM3usCaF_yQmje_atZGxJNJitAS--hAewoHgqoZFqCnDbYyXzjzKT5bW2ND7R4vyC4Yc4oRe7XHKQpG-2PKgThbHYnIMp4Iqk0yqsKLHfK-3TX0ZEteyOUWTKFGyy26a_CzLY99Ys4-kBSUFLHWyTRH0x5SmIMX4MMGVvABDyoCKw-PS8UN8RWZnxW6cRarOaSPF40FIkXTgZ-sDKiA4lZPgie4VWJjuP8NnWp1kcFKImzfdnCAZCLmA9poZnO7itTTw";
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
