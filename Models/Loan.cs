using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LoanServicingApi.Models.Enums;

namespace LoanServicingApi.Models
{
	public class Loan
	{
        [Key]
        public int Id { get; set; }
        public int BorrowerId { get; set; }
        public User? Borrower { get; set; }
        public int LoanOfficerId { get; set; }
        public User? LoanOfficer { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LoanStatus Status { get; set; }
        public List<LoanModification> Modifications { get; set; } = new List<LoanModification>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}

