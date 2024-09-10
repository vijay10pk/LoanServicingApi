using LoanServicingApi.Interfaces;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoanOfficerManagementController : ControllerBase
    {
        private readonly ILoanOfficerManagementRepository _loanOfficerManagementRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoanRepository _loanRepository;

        public LoanOfficerManagementController(ILoanOfficerManagementRepository loanOfficerManagementRepository, IUserRepository userRepository, ILoanRepository loanRepository)
        {
            _loanOfficerManagementRepository = loanOfficerManagementRepository;
            _userRepository = userRepository;
            _loanRepository = loanRepository;
        }

        [HttpGet]
        [Route("officers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllLoanOfficers()
        {
            var loanOfficers = await _userRepository.GetAllLoanOfficers();
            return Ok(loanOfficers);
        }

        [HttpGet]
        [Route("officers/{officerId}/loans")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOfficerLoans(int officerId)
        {
            try
            {
                var loans = await _loanOfficerManagementRepository.GetOfficersLoans(officerId);
                return Ok(loans);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("officers/loans/{loanId}/assign")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AssignOrReassignLoanToOfficer(int loanId, [FromBody] AssignLoanDto assignLoanDto)
        {
            try
            {
                var result = await _loanOfficerManagementRepository.AssignOrReassignLoanToOfficer(loanId, assignLoanDto.LoanOfficerId);
                if(result == null)
                {
                    return Ok("No changes were made. The loan is already assigned to this officer");
                }
                return Ok(new { Message = "Loan successfully assigned/reassigned", Modification = result });
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }

        }

        [HttpGet("officers/{officerId}/performance")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetOfficerPerformance(int officerId)
        {
            try
            {
                var performanceMetrics = await _loanOfficerManagementRepository.GetOfficerPerformance(officerId);
                return Ok(performanceMetrics);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }


    }
}
