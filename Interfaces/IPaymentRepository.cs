using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Interfaces
{
	public interface IPaymentRepository
	{
        Task<Payment> AddPayment(int loanId, AddPaymentDto paymentData);

    }
}

