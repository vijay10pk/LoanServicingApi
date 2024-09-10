using System;
namespace LoanServicingApi.Models.DTO
{
	public class AveragePerformanceDto
	{
        public double AverageLoansManaged { get; set; }
        public decimal AverageCollectionRate { get; set; }
        public double AverageCustomerSatisfactionScore { get; set; }
    }
}

