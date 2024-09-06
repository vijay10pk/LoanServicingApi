using System;
using LoanServicingApi.Models;

namespace LoanServicingApi.Interfaces
{
	public interface IJwtTokenService
	{
		string GenerateToken(User user);
	}
}

