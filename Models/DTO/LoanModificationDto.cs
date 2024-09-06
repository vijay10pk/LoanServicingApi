using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models.DTO
{
	public class LoanModificationDto
	{
        [Required]
        public decimal NewAmount { get; set; }

        [Required]
        public decimal NewInterestRate { get; set; }

        [Required]
        public DateTime NewEndDate { get; set; }
    }
}

