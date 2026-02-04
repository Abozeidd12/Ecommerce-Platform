using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.DTOs
{
    public class RegisterDTO
    {

        [Required(ErrorMessage = "Please enter your name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your username")]

        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid format for email address")]

        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]

        public string? Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password must be similar to password")]



        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your Phone number")]
        [Phone(ErrorMessage = "Please enter a valid Phone number")]

        public string? PhoneNumber { get; set; }
    }
}
