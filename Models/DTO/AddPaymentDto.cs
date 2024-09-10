using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models.DTO
{
	public class AddPaymentDto
	{
		[Required]
		public decimal Amount { get; set; }
	}
}

