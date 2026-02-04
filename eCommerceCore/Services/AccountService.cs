using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using eCommerceInfarsructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<(string? message,bool IsAuthenticated)> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return (  "Wrong Username or Password"
                ,  false );

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return ("Wrong Username or Password"
                , false);
            return  ( await _tokenService.GenerateToken(user) ,true);
        }

        public async Task<string> Register(RegisterDTO register)
        {
            ApplicationUser user = new ApplicationUser
            {
                Email = register.Email,
                UserName = register.Email,
                FullName = register.Name

            };

            var result = await _userManager.CreateAsync(user, register.Password);


            if (result.Succeeded)
                return "Succeded";
            else return $"Registeration failed : {result.Errors.Select(e => e.Description)} ";






        }

        public async Task<ReturnUsersDto?> GetUserBySsnAsync(Guid ssn)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
              u.Id == ssn.ToString()
                );

            if (user == null) return null;

            return new ReturnUsersDto
            {
                ssn = user.Id,
                username = user.UserName,
                name = user.FullName,
               
            };
        }


        public async Task UpdateUserAsync(Guid ssn, UpdateDTO dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == ssn.ToString());
               
            if (user == null) throw new KeyNotFoundException("User not found");

            // Update fields directly - NO RESTRICTIONS
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.FullName = dto.Name;

        

            

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }




    }
}
