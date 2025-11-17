using Microsoft.AspNetCore.Authorization;
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
			if (!ModelState.IsValid)
				return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

			// Check if email already exists
			var existingUser = await _authService.GetByEmailAsync(dto.Email);
			if (existingUser != null)
				return BadRequest(new { success = false, message = "Email already in use" });

			var user = await _authService.RegisterAsync(dto);
			return Ok(new { success = true, message = "Registration Successful", user });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

			var token = await _authService.LoginAsync(dto);
			if (token == null)
				return Unauthorized(new { success = false, message = "Invalid credentials" });

			return Ok(new { success = true, token });
		}
	}
}
