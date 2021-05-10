using System.Security.Claims;

namespace DAL.Contracts
{
    public interface IUserProvider
    {
        ClaimsPrincipal? CurrentUser { get;  }
        string CurrentName { get;  }
        string CurrentEmail { get;  }
    }
}