using System.ComponentModel.DataAnnotations;

namespace TeamSync.Application.DTOs.Project
{
	public class CreateProjectDto
	{
		[Required(ErrorMessage = "Name is required")]
		[MaxLength(100, ErrorMessage = "Name must be at most 100 characters")]
		public string Name { get; set; } = string.Empty;
		[MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
		public string Description { get; set; } = string.Empty;
	}
}

