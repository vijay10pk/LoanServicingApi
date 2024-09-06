using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanServicingApi.Helpers;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILogger<LoanController> _logger;
        private readonly ILoanRepository _loanRepository;
        private readonly IConfiguration _configuration;
        private readonly LoanServicingApiHelper _helper;

        public LoanController(ILogger<LoanController> logger, ILoanRepository loanRepository, LoanServicingApiHelper helper)
        {
            _logger = logger;
            _loanRepository = loanRepository;
            _helper = helper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Loan>))]
        public async Task<IActionResult> GetAllLoans()
        {
            var loans = await _loanRepository.GetAllLoans();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetLoanById(int Id)
        {
                var loan = await _loanRepository.GetLoanById(Id);
                return Ok(loan);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> CreateLoan([FromBody] Loan loanData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var borrowerExists = await _helper.UserExists(loanData.BorrowerId);
                if (!borrowerExists)
                {
                    return BadRequest("Invalid borrower ID");
                }

                var loanOfficerExists = await _helper.UserExists(loanData.LoanOfficerId);
                if (!loanOfficerExists)
                {
                    return BadRequest("Invalid loan officer ID");
                }
                var loan = new Loan
                {
                    Id = loanData.Id,
                    BorrowerId = loanData.BorrowerId,
                    LoanOfficerId = loanData.LoanOfficerId,
                    Amount = loanData.Amount,
                    InterestRate = loanData.InterestRate,
                    StartDate = loanData.StartDate,
                    EndDate = loanData.EndDate,
                    Status = Models.Enums.LoanStatus.Active
                };
                await _loanRepository.CreateLoan(loan);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin, LoanOfficer")]
        [HttpPost("{id}/modify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ModifyLoan(int id, [FromBody] LoanModificationDto modifiedLoanData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var modifiedLoan = new LoanModification
                {
                    NewAmount = modifiedLoanData.NewAmount,
                    NewInterestRate = modifiedLoanData.NewInterestRate,
                    NewEndDate = modifiedLoanData.NewEndDate,
                    ModifiedByUserId = _helper.GetCurrentUserId()
                };
                var createModification = await _loanRepository.ModifyLoan(id, modifiedLoan);

                return Ok(createModification);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteLoan(int Id)
        {
            await _loanRepository.DeleteLoan(Id);
            return Ok("Loan successfully deleted");
        }
    }
}
