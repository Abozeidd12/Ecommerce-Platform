using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceCore.DTOs
{
    public class LoginDTO
    {

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid format for email address")]

        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]

        public string? Password { get; set; }
    }
}
