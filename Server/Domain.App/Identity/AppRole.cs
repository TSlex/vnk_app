using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppRole : IdentityRole<long>
    {
        public string LocalizedName { get; set; } = default!;
    }
}