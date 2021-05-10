using Microsoft.AspNetCore.Identity;

namespace DAL.App.DTO.Identity
{
    public class AppUser : IdentityUser<long>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}