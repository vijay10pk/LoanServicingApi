using System;
using System.Security.Claims;
using LoanServicingApi.Data;
using LoanServicingApi.Helpers;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;
using LoanServicingApi.Models.Enums;
using Microsoft.EntityFrameworkCore;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Repositories
{
	public class LoanOfficerManagementRepository : ILoanOfficerManagementRepository
	{
        public readonly LoanServicingContext _context;
        public readonly IUserRepository _userRepository;
        public readonly ILoanRepository _loanRepository;
		public readonly LoanServicingApiHelper _helper;
        private IHttpContextAccessor _httpContextAccessor;


        public LoanOfficerManagementRepository(LoanServicingContext context, IUserRepository userRepository, ILoanRepository loanRepository, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_helper = new LoanServicingApiHelper(context, httpContextAccessor);
            _userRepository = userRepository;
            _loanRepository = loanRepository;
			
		}

		public Task<List<Loan>> GetOfficersLoans(int Id)
		{
			var loanOfficerExist = _helper.UserExists(Id);
			if(loanOfficerExist == null)
			{
				throw new NotFoundException("Loan officer doesn't exists");
			}
			var loans = _loanRepository.GetLoanByOfficerId(Id);
			return loans;
		}

        public async Task<LoanModification> AssignOrReassignLoanToOfficer(int loanId, int newLoanOfficerId)
        {
            var loan = await _loanRepository.GetLoanById(loanId);
            if (loan == null)
            {
                throw new NotFoundException("Loan not found");
            }

            var newLoanOfficer = await _userRepository.GetUserById(newLoanOfficerId);
            if (newLoanOfficer == null || newLoanOfficer.Role != UserRole.LoanOfficer)
            {
                throw new NotFoundException($"Loan Officer with ID {newLoanOfficerId} not found");
            }

            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(currentUserId, out int modifiedByUserId))
            {
                throw new UnauthorizedException("User not authenticated");
            }

            var modifiedBy = await _userRepository.GetUserById(modifiedByUserId);
            if (modifiedBy == null)
            {
                throw new NotFoundException($"User with ID {modifiedByUserId} not found");
            }

            if (loan.LoanOfficerId != newLoanOfficerId)
            {
                // Update the Loan's LoanOfficerId
                loan.LoanOfficerId = newLoanOfficerId;
                loan.Status = LoanStatus.Modified;

                var modification = new LoanModification
                {
                    LoanId = loanId,
                    ModificationDate = DateTime.UtcNow,
                    NewLoanOfficerId = newLoanOfficerId,
                    ModifiedByUserId = modifiedByUserId,
                    ModifiedBy = modifiedBy
                };

                _context.LoanModifications.Add(modification);
                _context.Loans.Update(loan);

                await _context.SaveChangesAsync();

                return modification;

            }
            else
            {
                return null;
            }
        }

        public async Task<OfficerPerformanceMetricsDto> GetOfficerPerformance(int officerId)
        {
            var officer = await _userRepository.GetUserById(officerId);
            if(officer == null || officer.Role != UserRole.LoanOfficer)
            {
                throw new NotFoundException("Loan officer not found");
            }
            var performances = await _context.LoanOfficerPerformances.Where(p => p.LoanOfficerId == officerId).ToListAsync();

            if (!performances.Any())
            {
                throw new NotFoundException($"No performance data found for Loan Officer {officerId}.");
            }

            var latestPerformance = performances.OrderByDescending(p => p.PerformanceDate).First();

            var averagePerformance = new AveragePerformanceDto
            {
                AverageLoansManaged = Math.Round(performances.Average(p => p.LoansManaged),2),
                AverageCollectionRate = Math.Round(performances.Average(p => p.CollectionRate),2),
                AverageCustomerSatisfactionScore = Math.Round(performances.Average(p => p.CustomerSatisfactionScore),2)
            };

            var totalLoansManaged = await _context.Loans.CountAsync(l => l.LoanOfficerId == officerId);
            var activeLoansCount = await _context.Loans.CountAsync(l => l.LoanOfficerId == officerId && l.Status == LoanStatus.Active);
            var totalLoanAmount = await _context.Loans.Where(l => l.LoanOfficerId == officerId).SumAsync(l => l.Amount);

            return new OfficerPerformanceMetricsDto
            {
                OfficerId = officerId,
                OfficerName = officer.Fullname,
                LatestPerformance = latestPerformance,
                AveragePerformance = averagePerformance,
                TotalLoansManaged = totalLoansManaged,
                ActiveLoansCount = activeLoansCount,
                TotalLoanAmount = totalLoanAmount
            };

        }
	}
}

