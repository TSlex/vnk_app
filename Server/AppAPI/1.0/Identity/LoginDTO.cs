using System.ComponentModel.DataAnnotations;

namespace AppAPI._1._0.Identity
{
    public class LoginDTO
    {
        [MaxLength(50)]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [MaxLength(50)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}