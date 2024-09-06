using System;
using System.ComponentModel.DataAnnotations;
using LoanServicingApi.Models.Enums;

namespace LoanServicingApi.Models
{
	public class User
	{
        [Key]
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}

