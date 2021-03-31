using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;
using PublicApi.v1.Enums;

namespace PublicApi.v1
{
    public class AttributeTypeGetDTO
    {
        public string Name { get; set; } = default!;

        public AttributeDataType DataType { get; set; } = default!;

        public ICollection<string>? Values { get; set; }
    }
    
    public class AttributeTypeGetDetailsDTO
    {
        public string Name { get; set; } = default!;
        public long UsedCount { get; set; }

        public string DefaultCustomValue { get; set; } = default!;
        
        public bool PreDefinedValues { get; set; }

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