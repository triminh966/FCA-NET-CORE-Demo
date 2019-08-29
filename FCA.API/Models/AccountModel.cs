using System.ComponentModel.DataAnnotations;
using FCA.API.Resources;

namespace FCA.API.Models
{
    public class AccountLoginModel
    {
        [Required(ErrorMessageResourceName = ResourceKeys.ValidationRequired, ErrorMessageResourceType = typeof(DefinitionResources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = ResourceKeys.ValidationRequired, ErrorMessageResourceType = typeof(DefinitionResources))]
        public string Username { get; set; }

        public string Email { get; set; }
    }

    /// <summary>
    /// Account refresh token model.
    /// </summary>
    public class AccountRefreshTokenModel
    {
        /// <summary>
        /// Gets or sets RefreshToken
        /// </summary>
        [Required(ErrorMessageResourceName = ResourceKeys.ValidationRequired, ErrorMessageResourceType = typeof(DefinitionResources))]
        public string RefreshToken { get; set; }
    }

    public class AccountSignUpModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmedPassword { get; set; }
    }

    public class AccountSignOutModel
    {
        [Required(ErrorMessageResourceName = ResourceKeys.ValidationRequired, ErrorMessageResourceType = typeof(DefinitionResources))]
        public string Username { get; set; }
    }

    public class AccountLoginResponseModel
    {
        /// <summary>
        /// Gets and Sets the AccessToken.
        /// Pattern: [A-Za-z0-9-_=.]+
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets and sets the Id Token
        /// Pattern: [A-Za-z0-9-_=.]+
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Gets and sets the expiration period of
        /// the authentication result in seconds.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets and sets the Refresh Token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets and sets The TokenType.
        /// </summary>
        public string TokenType { get; set; }
    }
}
