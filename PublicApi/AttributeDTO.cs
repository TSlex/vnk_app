using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi.v1.Identity
{
    public class AttributeGetDTO
    {
        public bool Displayed { get; set; }

        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;

        public ICollection<string>? Values { get; set; }
    }

    public class AttributePostDTO
    {
        [Required] public bool Displayed { get; set; }

        [Required] public string Name { get; set; } = default!;

        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributePutDTO : DomainEntityId
    {
        [Required] public bool Displayed { get; set; }

        [Required] public string Name { get; set; } = default!;
    }
}