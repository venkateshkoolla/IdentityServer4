using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace IdentityServer.Infrastructure.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

    }
}
