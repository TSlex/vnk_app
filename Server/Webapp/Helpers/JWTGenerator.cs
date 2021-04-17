using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Helpers
{
    public static class JWTGenerator
    {
        public static string GenerateJWT(IEnumerable<Claim> claims, string signingKey, string issuer, string audience, int expiresInDays)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var singingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                null,
                DateTime.UtcNow.AddDays(expiresInDays),
                singingCredentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}