using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models.DTO
{
	public class AssignLoanDto
	{
		[Required]
		public int LoanOfficerId { get; set; }
	}
}

