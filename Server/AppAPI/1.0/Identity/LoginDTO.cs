using System.ComponentModel.DataAnnotations;

namespace AppAPI._1._0.Identity
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}