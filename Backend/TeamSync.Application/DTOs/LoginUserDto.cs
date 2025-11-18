using System.ComponentModel.DataAnnotations;

namespace TeamSync.Application.DTOs
{
	public class LoginUserDto
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }
	}
}
