using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace HonuTasks.Extensions
{
    public static class IdentityExtensions
    {
        public static int? GetCreatorId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CreatorId");
            return (claim != null) ? int.Parse(claim.Value) : null;
        }

    }
}
