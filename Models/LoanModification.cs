using System;
using System.ComponentModel.DataAnnotations;
namespace LoanServicingApi.Models
{
	public class LoanModification
	{
        [Key]
        public int Id { get; set; }
        public int LoanId { get; set; }
        public DateTime ModificationDate { get; set; }
        public int NewLoanOfficerId { get; set; }
        public decimal NewInterestRate { get; set; }
        public decimal NewAmount { get; set; }
        public DateTime NewEndDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public User ModifiedBy { get; set; }
    }
}

