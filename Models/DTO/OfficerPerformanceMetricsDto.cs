using System;

namespace LoanServicingApi.Models.DTO
{
	public class OfficerPerformanceMetricsDto
	{
        public int OfficerId { get; set; }
        public string OfficerName { get; set; }
        public LoanOfficerPerformance LatestPerformance { get; set; }
        public AveragePerformanceDto AveragePerformance { get; set; }
        public int TotalLoansManaged { get; set; }
        public int ActiveLoansCount { get; set; }
        public decimal TotalLoanAmount { get; set; }
    }
}

