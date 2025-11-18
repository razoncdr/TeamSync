using Microsoft.AspNetCore.Mvc;
using TeamSync.Application.DTOs;
using TeamSync.Application.Interfaces.Services;

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

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
		{
			var user = await _authService.RegisterAsync(dto);
			return Ok(new { success = true, message = "Registration Successful", user });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
		{
			var token = await _authService.LoginAsync(dto);
			return Ok(new { success = true, token });
		}
	}
}
