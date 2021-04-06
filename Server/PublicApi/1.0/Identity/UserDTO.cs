using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi.v1.Identity
{
    public class UserGetDTO: DomainEntityId
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Role { get; set; }
    }

    public class UserPostDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(128)]
        public string FirstName { get; set; } = default!;

        [Required]
        [MinLength(1)]
        [MaxLength(128)]
        public string LastName { get; set; } = default!;

        [Required] [EmailAddress] public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
    
    public class UserPatchDTO: DomainEntityId
    {
        [Required]
        [MinLength(1)]
        [MaxLength(128)]
        public string FirstName { get; set; } = default!;

        [Required]
        [MinLength(1)]
        [MaxLength(128)]
        public string LastName { get; set; } = default!;

        [Required] [EmailAddress] public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}