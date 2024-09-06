using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models.DTO
{
	public class UpdateUserDto
	{
		public string Fullname { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public string PasswordHash { get; set; }
	}
}

