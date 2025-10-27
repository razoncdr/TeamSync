using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthApp.Domain.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = "Unknown";
		public string Email { get; set; } = String.Empty;
		public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
		public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
	}
}
