﻿using System.Linq;
using System.Security.Claims;
using Contracts.Domain;
using Microsoft.AspNetCore.Http;

namespace WebApp.Helpers
{
    public class UserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string CurrentName => _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "";

        public string CurrentEmail => _httpContextAccessor.HttpContext?.User.Claims
            .Single(claim => claim.Type == ClaimTypes.Email).Value ?? "";
    }
}