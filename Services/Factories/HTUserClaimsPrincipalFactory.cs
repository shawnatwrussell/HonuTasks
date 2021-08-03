using HonuTasks.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HonuTasks.Services.Factories
{
    public class HTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<HTUser, IdentityRole>
    {
        public HTUserClaimsPrincipalFactory(
    UserManager<HTUser> userManager, RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor) :
    base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(HTUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("UserId", user.ToString()));

            return identity;
        }

    }
}
