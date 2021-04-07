using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi.v1.Identity
{
    public class UserGetDTO : DomainEntityId
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string RoleLocalized { get; set; } = default!;
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

        [MaxLength(128)] public string? Role { get; set; }
    }

    public class UserPatchDTO : DomainEntityId
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
    }

    public class UserPasswordPatchDTO : DomainEntityId
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = default!;
    }

    public class UserRolePatchDTO : DomainEntityId
    {
        [Required] [MaxLength(128)] public string Role { get; set; } = default!;
    }
}