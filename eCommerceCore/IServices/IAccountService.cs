using eCommerceCore.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.IServices
{
    public interface IAccountService
    {
        Task<string> Register(RegisterDTO register);

        Task<(string? message, bool IsAuthenticated)> Login(LoginDTO loginDto);

        Task<ReturnUsersDto?> GetUserBySsnAsync(Guid ssn);
        
        Task UpdateUserAsync(Guid ssn, UpdateDTO dto);
       

    }
}
