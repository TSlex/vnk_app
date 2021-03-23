using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;
using Domain.Enums;

namespace PublicApi
{
    public class AttributeTypeGetDTO
    {
        public string Name { get; set; } = default!;

        public AttributeDataType DataType { get; set; } = default!;

        public ICollection<string>? Values { get; set; }
    }

    public class AttributeTypePostDTO
    {
        [Required] public string Name { get; set; } = default!;
        [Required] public AttributeDataType DataType { get; set; } = default!;

        public ICollection<string>? Values { get; set; }
    }

    public class AttributeTypePutDTO : DomainEntityId
    {
        [Required] public string Name { get; set; } = default!;
    }
}