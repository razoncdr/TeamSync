using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace TeamSync.API.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}


		[HttpGet]
		public IActionResult Index()
		{
			return Ok("Good");
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto userInfo)
		{
			Console.WriteLine(userInfo);
			var user = await _authService.RegisterAsync(userInfo);
			return Ok(new { success = true, message = "Registration Successful", newUser = user });
		}
	}
}

