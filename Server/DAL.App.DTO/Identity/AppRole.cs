
using Microsoft.AspNetCore.Identity;

namespace DAL.App.DTO.Identity
{
    public class AppRole : IdentityRole<long>
    {
        public string LocalizedName { get; set; } = default!;
    }
}