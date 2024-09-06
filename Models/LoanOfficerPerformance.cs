using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models
{
	public class LoanOfficerPerformance
	{
        [Key]
        public int Id { get; set; }
        public int LoanOfficerId { get; set; }
        public DateTime PerformanceDate { get; set; }
        public int LoansManaged { get; set; }
        public decimal CollectionRate { get; set; }
        public int CustomerSatisfactionScore { get; set; }
    }
}

