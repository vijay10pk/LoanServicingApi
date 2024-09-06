using System;
using System.Security.Claims;
using LoanServicingApi.Data;
using LoanServicingApi.Models;
using LoanServicingApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanServicingApi.Helpers
{
	public class LoanServicingApiHelper
	{
        private static IServiceProvider _serviceProvider;
        private LoanServicingContext _context;
        private IHttpContextAccessor _httpContextAccessor;

        public LoanServicingApiHelper(LoanServicingContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<bool> EmailExists(string email)
        {

            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByEmail(string email) => await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public int GetCurrentUserId()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Int32.Parse(currentUserId);
        }
    }
}

