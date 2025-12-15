using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSync.Domain.Entities;

namespace TeamSync.Application.DTOs.Task
{
	public class UpdateTaskDto
	{
		[Required(ErrorMessage = "Title is required")]
		[MaxLength(100, ErrorMessage = "Title must be at most 100 characters")]
		public string Title { get; set; } = string.Empty;
		[MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters")]
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public List<string> AssignedMemberIds { get; set; } = new();
		[EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status")]
		public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
		public DateTime CreatedAt { get; set; }
	}
}
