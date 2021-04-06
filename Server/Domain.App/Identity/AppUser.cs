using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser<long>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public bool Protected { get; set; }
    }
}