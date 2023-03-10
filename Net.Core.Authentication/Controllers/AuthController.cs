using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.Core.Authentication.Enum;
using Net.Core.Authentication.Exceptions;
using Net.Core.Authentication.Models;
using Net.Core.Authentication.Services;
using System.Threading.Tasks;

namespace Net.Core.Authentication.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ValidateModelAttribute), Order = 1)] // if u need add in controller
    [Route("auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(Register model)
        {
            var userExists = await authService.FindByUserName(model.Username);
            if (userExists != null)
                throw new AppException("User already exists!");

            var result = await authService.CreateUser(model);
            if (!result.Status.Equals(ResponseCode.Ok))
                throw new AppException("User creation failed!");

            return Ok(new Response { Status = ResponseCode.Ok, Message = "User creation successfully!" });
        }

        //[AllowAnonymous]
        [BasicAuth] // You can optionally provide a specific realm --> [BasicAuth("my-realm")]
        [HttpPost]
        [Route("get-token")]
        public async Task<IActionResult> GetToken(AccessToken request)
        {
            var user = await authService.ValidateAccessToken(request);
            var response = await authService.CreateToken(user);
            return Ok(response);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> Refresh(RefreshToken request)
        {
            //string access_token = this.User.FindFirstValue("access_token");
            var user = await authService.ValidateRefreshToken(request);
            var response = await authService.CreateToken(user);
            return Ok(response);
        }

        [HttpPost]
        [Route("test-auth")]
        //[ValidateModel] //if u need add in action
        public async Task<IActionResult> Auth(Request request)
        {
            //string access_token = this.User.FindFirstValue("access_token");
            var response = await authService.Auth(request);
            return Ok(response);
        }
    }
}
