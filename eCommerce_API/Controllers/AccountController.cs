using eCommerceCore.DTOs;
using eCommerceCore.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            var message = await _accountService.Register(register);
            return Ok(message);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var result = await _accountService.Login(login);

            if (!result.IsAuthenticated)
                return Unauthorized(result.message);


            SetAccessTokenInCookie(result.message!);


            
           
            
                
                return Ok("LoggedIn Successfully");
            
           
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "Logged out successfully" });
        }


        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var token = Request.Cookies["refreshToken"];
        //    if (string.IsNullOrEmpty(token)) return BadRequest("No refresh token found");

        //    var result = await _accountService.RevokeTokenAsync(token);
        //    Response.Cookies.Delete("refreshToken");

        //    if (!result) return BadRequest("Invalid token or already revoked");
        //    return Ok("Logged out successfully");
        //}


        [HttpGet("User/{ssn}")]
        public async Task<IActionResult> GetUser(Guid ssn)
        {
            var user = await _accountService.GetUserBySsnAsync(ssn);
            if (user == null) return NotFound();
            return Ok(user);
        }

     

        [HttpPut("Update/{ssn:guid}")]
        public async Task<IActionResult> UpdateUser(Guid ssn, [FromBody] UpdateDTO dto)
        {
            try
            {
                var adminSsnClaim = User.FindFirst("ssn")?.Value;
                var adminSsn = string.IsNullOrEmpty(adminSsnClaim)
                    ? Guid.Empty
                    : Guid.Parse(adminSsnClaim);

                await _accountService.UpdateUserAsync(ssn, dto);

                return Ok(new { message = "User updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {SSN}", ssn);
                return NotFound(new { message = "User not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during user update");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser failed for SSN: {SSN}", ssn);
                return StatusCode(500, new { message = "An error occurred while updating user" });
            }
        }


        


        private void SetAccessTokenInCookie(string token)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,        
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            });
        }

    }
}
