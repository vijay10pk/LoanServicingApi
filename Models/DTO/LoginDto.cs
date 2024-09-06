using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models.DTO
{
	public class LoginDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string PasswordHash { get; set; }
	}
}

