using eCommerce.Core.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public AuthController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            AuthenticationResponse? authResponse = await _usersService.Login(loginRequest);
            if (authResponse == null)
            {
                return BadRequest("Invalid email or password");
            }
            return Ok(authResponse);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            AuthenticationResponse? authResponse = await _usersService.Register(registerRequest);
            if (authResponse == null)
            {
                return Unauthorized("Unable to register user");
            }
            return Ok(authResponse);
        }
    }
}
