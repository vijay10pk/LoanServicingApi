using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Interfaces
{
	public interface IUserRepository
	{
        Task<User> EmailExists(string email);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> UpdateUser(UpdateUserDto updatedUserData);
        Task<User> UpdateUserById(int Id, UpdateUserDto updatedUserData);
        Task DeleteUser(int id);
    }
}

