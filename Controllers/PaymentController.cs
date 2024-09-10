using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpPost("loan/{loanId}")]
        public async Task<ActionResult> AddPayment(int loanId, [FromBody] AddPaymentDto paymentData)
        {
            try
            {
                var newPayment = await _paymentRepository.AddPayment(loanId, paymentData);
                return Ok(newPayment);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"A error occured while processesing the request: {ex.Message}");
            }

        }
    }
}
