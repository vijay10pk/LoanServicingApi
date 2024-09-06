using System;
using LoanServicingApi.Data;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models;
using LoanServicingApi.Helpers;
using LoanServicingApi.Models.DTO;
using LoanServicingApi.Services;
using Microsoft.Extensions.Configuration;

namespace LoanServicingApi.Repositories
{
	public class AuthenticationRepository : IAuthenticationRepository
	{
		private LoanServicingContext _context;
		private UserRepository _userRepository;
		private LoanServicingApiHelper _helper;
		private IConfiguration _configuration;
		private IJwtTokenService _jwtTokenService;
		private IHttpContextAccessor _httpContextAccessor;
		

		public AuthenticationRepository(LoanServicingContext context, IJwtTokenService jwtTokenService, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_helper = new LoanServicingApiHelper(context,httpContextAccessor);
			_jwtTokenService = jwtTokenService;

        }

		public async Task<User> RegisterUser(User userData)
		{
			if(await _helper.EmailExists(userData.Email))
			{
				throw new ApplicationException("Email already in use");
			}

			_context.Users.Add(userData);
			await _helper.Save();
			return userData;
		}

		public async Task<string> Login(LoginDto userCredentials)
		{
            // Find the user by email
            var user = await _helper.GetUserByEmail(userCredentials.Email);
			if (user == null)
				return null;
			if (!BCrypt.Net.BCrypt.Verify(userCredentials.PasswordHash, user.PasswordHash))
				return null;
            // Generate a JWT token
            var token = _jwtTokenService.GenerateToken(user);
			return token;
        }
	}
}

