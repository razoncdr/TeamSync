using AuthApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.API.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService) {
			_authService = authService;
		}


		[HttpGet]
		public IActionResult Index()
		{
			return Ok("Good");
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var user = await _authService.RegisterAsync(request);
			return Ok(new { success = true, message = "Registration Successful", user = user});
		}
	}
}

