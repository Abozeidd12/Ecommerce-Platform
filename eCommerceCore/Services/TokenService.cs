using eCommerceCore.IServices;
using eCommerceInfarsructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(ApplicationUser applicationUser)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));

            List<Claim> claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, applicationUser.FullName.ToString()),
                new Claim(ClaimTypes.Email,applicationUser.Email.ToString()),
                new Claim(ClaimTypes.NameIdentifier,applicationUser.Id)

            };

            var id = applicationUser.Id;

            if (id != null)
            {
                claim.Add(new Claim("ID", id));
            }

            var roles = await _userManager.GetRolesAsync(applicationUser);

            foreach( var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(claims: claim, issuer: _configuration["JWT:Issuer"], audience: _configuration["JWT:Audience"], expires: DateTime.UtcNow.AddMinutes(15),

                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
