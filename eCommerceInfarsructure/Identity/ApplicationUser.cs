using eCommerceDomain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceInfarsructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;

        public Cart? Cart { get; set; }
    }
}
