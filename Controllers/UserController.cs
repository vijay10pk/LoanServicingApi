using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LoanServicingApi.Data;
using LoanServicingApi.Models;
using LoanServicingApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LoanServicingApi.Models.Enums;
using LoanServicingApi.Models.DTO;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IUserRepository usersRepository)
        {
            _logger = logger;
            _userRepository = usersRepository;
        }

        //[Authorize(Roles = "Admin") ]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<User>))]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                return Ok(user);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto updatedUserData)
        {
            try
            {
                await _userRepository.UpdateUser(updatedUserData);
                return Ok();
                ;
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                await _userRepository.DeleteUser(id);
                return NoContent();
            }
            catch(Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
