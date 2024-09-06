using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;
using LoanServicingApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace LoanServicingApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationRepository authenticationRepository)
        {
            _logger = logger;
            _authenticationRepository = authenticationRepository;
        }

        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegisterUser(User userData)
        {
            try
            {
                var user = new User
                {
                    Fullname = userData.Fullname,
                    Email = userData.Email,
                    PasswordHash = BC.HashPassword(userData.PasswordHash),
                    Role = Models.Enums.UserRole.Borrower,
                    CreatedAt = DateTime.UtcNow
                };
                await _authenticationRepository.RegisterUser(user);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
            
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(LoginDto userCredentials)
        {
            var token = await _authenticationRepository.Login(userCredentials);
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Invalid email or password");

            return Ok(new { token });
        }
    }
}
