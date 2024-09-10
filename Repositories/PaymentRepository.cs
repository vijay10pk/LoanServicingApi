using System;
using LoanServicingApi.Data;
using LoanServicingApi.Helpers;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Repositories
{
	public class PaymentRepository : IPaymentRepository
	{
		private readonly LoanServicingContext _context;
		private readonly ILoanRepository _loanRepository;
		private readonly LoanServicingApiHelper _helper;

		public PaymentRepository(LoanServicingContext context, ILoanRepository loanRepository, LoanServicingApiHelper helper)
		{
			_context = context;
			_loanRepository = loanRepository;
			_helper = helper;
		}

		public async Task<Payment> AddPayment(int loanId, AddPaymentDto paymentData)
		{
			var loan = await _loanRepository.GetLoanById(loanId);
			if (loan == null) throw new NotFoundException("Loan data not found");

			var newPayment = new Payment
			{
				LoanId = loan.Id,
				Amount = paymentData.Amount,
				PaymentDate = DateTime.UtcNow
			};

			_context.Payments.Add(newPayment);
			await _helper.Save();
			return newPayment;
		}
	}
}

