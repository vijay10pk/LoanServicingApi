using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Interfaces
{
	public interface ILoanRepository
	{
        Task<List<Loan>> GetAllLoans();
        Task<Loan> GetLoanById(int id);
        Task<Loan> CreateLoan(Loan loanData);
        Task<LoanModification> ModifyLoan(int loanId, LoanModification modifiedLoanData);
        Task DeleteLoan(int Id);
        Task<bool> LoanExists(int Id);
    }
}

