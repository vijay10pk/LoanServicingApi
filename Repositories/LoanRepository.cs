using System;
using LoanServicingApi.Data;
using LoanServicingApi.Helpers;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.Enums;
using Microsoft.EntityFrameworkCore;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private LoanServicingContext _context;
        private readonly LoanServicingApiHelper _helper;
        private IHttpContextAccessor _httpContextAccessor;

        public LoanRepository(LoanServicingContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _helper = new LoanServicingApiHelper(context, httpContextAccessor);
        }

        public async Task<List<Loan>> GetAllLoans()
        {
            return await _context.Loans.ToListAsync();
        }

        public async Task<Loan> GetLoanById(int Id)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == Id);

            if (loan == null)
            {
                throw new NotFoundException("Loan Details not found");
            }

            return loan;
        }

        public async Task<Loan> CreateLoan(Loan loanData)
        {
            _context.Add(loanData);
            await _helper.Save();
            return loanData;
        }

        public async Task<LoanModification> ModifyLoan(int loanId, LoanModification modifiedLoanData)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("Loan not found");
            }

            var modifiedBy = await _context.Users.FindAsync(modifiedLoanData.ModifiedByUserId);
            if (modifiedBy == null)
            {
                throw new NotFoundException($"User with ID {modifiedLoanData.ModifiedByUserId} not found");
            }

            var entry = _context.Entry(loan);
            bool isModified = false;

            if (modifiedLoanData.NewAmount != 0 && modifiedLoanData.NewAmount != loan.Amount)
            {
                entry.Property(l => l.Amount).CurrentValue = modifiedLoanData.NewAmount;
                entry.Property(l => l.Amount).IsModified = true;
                isModified = true;
            }

            if (modifiedLoanData.NewInterestRate != 0 && modifiedLoanData.NewInterestRate != loan.InterestRate)
            {
                entry.Property(l => l.InterestRate).CurrentValue = modifiedLoanData.NewInterestRate;
                entry.Property(l => l.InterestRate).IsModified = true;
                isModified = true;
            }

            if (modifiedLoanData.NewEndDate != DateTime.MinValue && modifiedLoanData.NewEndDate != loan.EndDate)
            {
                entry.Property(l => l.EndDate).CurrentValue = modifiedLoanData.NewEndDate;
                entry.Property(l => l.EndDate).IsModified = true;
                isModified = true;
            }

            if (isModified)
            {
                entry.Property(l => l.Status).CurrentValue = LoanStatus.Modified;
                entry.Property(l => l.Status).IsModified = true;

                var modification = new LoanModification
                {
                    LoanId = loanId,
                    ModificationDate = DateTime.UtcNow,
                    NewAmount = modifiedLoanData.NewAmount,
                    NewInterestRate = modifiedLoanData.NewInterestRate,
                    NewEndDate = modifiedLoanData.NewEndDate,
                    ModifiedByUserId = modifiedLoanData.ModifiedByUserId,
                    ModifiedBy = modifiedBy
                };

                _context.LoanModifications.Add(modification);
                await _context.SaveChangesAsync();
                return modification;
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteLoan(int Id)
        {
            var loan = await GetLoanById(Id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
                await _helper.Save();
            }
        }

        public async Task<bool> LoanExists(int Id)
        {
            return await _context.Loans.AnyAsync(l => l.Id == Id);
        }
    }
}

