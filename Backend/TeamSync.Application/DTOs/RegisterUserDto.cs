using System.ComponentModel.DataAnnotations;

namespace TeamSync.Application.DTOs
{
	public class RegisterUserDto
	{
		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
		public string Password { get; set; }
	}
}
