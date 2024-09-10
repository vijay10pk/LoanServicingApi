using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Interfaces
{
	public interface ILoanOfficerManagementRepository
	{
        Task<List<Loan>> GetOfficersLoans(int Id);
        Task<LoanModification> AssignOrReassignLoanToOfficer(int loanId, int newLoanOfficerId);
        Task<OfficerPerformanceMetricsDto> GetOfficerPerformance(int officerId);
    }
}

