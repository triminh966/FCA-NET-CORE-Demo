using System;
using System.Threading.Tasks;
using FCA.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = FCA.API.Services.IAuthenticationService;

namespace FCA.API.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
       private readonly IAuthenticationService _authenticationService;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        #endregion

        /// <summary>
        /// Account Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/signin")]
        [AllowAnonymous]
        public async Task<ActionResult<AccountLoginResponseModel>> Login([FromBody] AccountLoginModel model)
        {
            Console.WriteLine(@"AccountController: Login Function");
            if (!ModelState.IsValid || model == null)
                return BadRequest(nameof(model));

            var response = await _authenticationService.Signin(model);

            return Ok(response);
        }

        [HttpPost]
        [Route("api/register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] AccountSignUpModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(nameof(model));

            // Register User.

            var response = _authenticationService.Register(model);

            if (response.IsFaulted)
                return BadRequest(response.Exception);

            return Ok(response.Result);
        }

        [HttpPost]
        [Route("api/refresh-token")]
        public IActionResult RefreshToken([FromBody] AccountRefreshTokenModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(nameof(model));

            var response = _authenticationService.RefreshToken(model);

            return Ok(response);
        }

        [HttpPost]
        [Route("api/signout")]
        [AllowAnonymous]
        public IActionResult SignOut([FromBody] AccountSignOutModel accountSignOutModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(nameof(accountSignOutModel));

            _authenticationService.SignOut(accountSignOutModel);
            return Ok();
        }
    }
}