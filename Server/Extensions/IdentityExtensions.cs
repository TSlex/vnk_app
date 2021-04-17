using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Extensions
{
    public static class IdentityExtensions
    {
        public static long UserId(this ClaimsPrincipal user)
        {
            var stringId = user.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            return long.Parse(stringId);
        }
    }
}