using System;
using LoanServicingApi.Models;
using LoanServicingApi.Models.DTO;

namespace LoanServicingApi.Interfaces
{
	public interface IAuthenticationRepository
	{
        Task<User> RegisterUser(User userData);
        Task<String> Login(LoginDto userCredentials);

    }
}

