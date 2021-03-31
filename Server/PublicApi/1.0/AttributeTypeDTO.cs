using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Base;
using PublicApi.v1.Enums;

namespace PublicApi.v1
{
    public class AttributeTypeGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public long UsedCount { get; set; }
        
        public AttributeDataType DataType { get; set; } = default!;

        public bool SystemicType { get; set; }
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
    }
    
    public class AttributeTypeGetDetailsDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public long UsedCount { get; set; }

        public string DefaultCustomValue { get; set; } = default!;
        
        public AttributeDataType DataType { get; set; } = default!;

        public bool SystemicType { get; set; }
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
        
        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }
        
        public long ValuesCount { get; set; }
        public long UnitsCount { get; set; }

        public ICollection<AttributeTypeValueDetailsDTO>? Values { get; set; }
        public ICollection<AttributeTypeUnitsDetailsDTO>? Units { get; set; }
    }

    public class AttributeTypeValueDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }
    
    public class AttributeTypeUnitsDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeTypePostDTO
    {
        [Required] public string Name { get; set; } = default!;
        
        public string DefaultCustomValue { get; set; } = default!;
        
        [Required] public AttributeDataType DataType { get; set; } = default!;
        
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
        
        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }

        public ICollection<string>? Values { get; set; }
        public ICollection<string>? Units { get; set; }
    }

    public class AttributeTypePutDTO : DomainEntityId
    {
        [Required] public string Name { get; set; } = default!;
        
        public string DefaultCustomValue { get; set; } = default!;
        
        [Required] public AttributeDataType DataType { get; set; } = default!;
        
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
        
        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }
    }
}