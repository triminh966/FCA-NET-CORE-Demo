using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using FCA.API.Models;
using FCA.Core.Secrets;

namespace FCA.API.Services
{
    public interface IAuthenticationService
    {
        Task<AccountLoginResponseModel> Signin(AccountLoginModel model);

        void SignOut(AccountSignOutModel model);

        string RefreshToken(AccountRefreshTokenModel model);

        Task<SignUpResponse> Register(AccountSignUpModel model);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;
        private readonly IFcaSecrets _secret;
        
        #region Constructor

        /// <summary>
        /// Instantiates <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="secret">Secret retrieved from SecretManager.</param>
        public AuthenticationService(IFcaSecrets secret)
        {
            _secret = secret;
        }

        #endregion

        public async Task<AccountLoginResponseModel> Signin(AccountLoginModel model)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_secret.AwsAccessKey, _secret.AwsSecretKey, _region);
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _secret.UserPoolId,
                ClientId = _secret.ClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", model.Username);
            request.AuthParameters.Add("PASSWORD", model.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return Mapper.Map<AccountLoginResponseModel>(response);
        }

        public void SignOut(AccountSignOutModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username))
                throw new ArgumentException(nameof(model));

            throw new NotImplementedException();
        }

        public string RefreshToken(AccountRefreshTokenModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<SignUpResponse> Register(AccountSignUpModel model)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_secret.AwsAccessKey, _secret.AwsSecretKey, _region);
            var request = new SignUpRequest
            {
                ClientId = _secret.ClientId,
                Password = model.Password,
                Username = model.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = model.Email
            };

            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            return response;
        }
    }
}
