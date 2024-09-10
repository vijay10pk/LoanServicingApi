using System;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Data;
using LoanServicingApi.Models;
using LoanServicingApi.Models.Enums;
using LoanServicingApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using LoanServicingApi.Helpers;
using static LoanServicingApi.Exceptions.LoanServicingExceptions;

namespace LoanServicingApi.Repositories
{
	public class UserRepository : IUserRepository
	{
		private LoanServicingContext _context;
        private LoanServicingApiHelper _helper;
        private IHttpContextAccessor _httpContextAccessor;

		public UserRepository(LoanServicingContext context, IHttpContextAccessor httpContextAccessor)
        {
			_context = context;
            _helper = new LoanServicingApiHelper(context, httpContextAccessor);
		}

        public async Task<User> EmailExists(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("User not found");

            return user;
        }

        public async Task<List<User>> GetAllLoanOfficers()
        {
            var loanOfficers = await _context.Users.Where(u => u.Role == UserRole.LoanOfficer).ToListAsync();
            return loanOfficers;
        }

        public async Task<User> UpdateUser(UpdateUserDto updatedUserData)
        {
            var user = await _helper.GetUserByEmail(updatedUserData.Email);

            if(user == null)
            {
                throw new NotFoundException("User not found");
            }
            return await UpdateUserById(user.Id, updatedUserData);
        }

        public async Task<User> UpdateUserById(int Id, UpdateUserDto updatedUserData)
        {
            var existingUser = await _context.Users.FindAsync(Id);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!string.IsNullOrEmpty(updatedUserData.Fullname))
                existingUser.Fullname = updatedUserData.Fullname;
            if (!string.IsNullOrEmpty(updatedUserData.Email))
                existingUser.Email = updatedUserData.Email;
            if (!string.IsNullOrEmpty(updatedUserData.PasswordHash))
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUserData.PasswordHash);

            _context.Entry(existingUser).State = EntityState.Modified;

            _context.Entry(existingUser).Property(u => u.CreatedAt).IsModified = false;
            _context.Entry(existingUser).Property(u => u.Role).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("The user was modified by another process");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the user", ex);
            }

            return existingUser;

        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _helper.Save();
            }

        }

    }
}

