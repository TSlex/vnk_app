using Microsoft.AspNetCore.Identity;

namespace DAL.App.Entities.Identity
{
    public class AppRole : IdentityRole<long>
    {
        public string LocalizedName { get; set; } = default!;
    }
}