using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Base;
using DAL.Base.Entities;
using PublicApi._1._0.Enums;

namespace PublicApi._1._0
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

        public ICollection<AttributeTypeValueDetailsDTO> Values { get; set; } = default!;
        public ICollection<AttributeTypeUnitDetailsDTO> Units { get; set; } = default!;
    }

    public class AttributeTypeValueDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }
    
    public class AttributeTypeUnitDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeTypePostDTO
    {
        [Required] public string Name { get; set; } = default!;
        
        public string? DefaultCustomValue { get; set; }
        
        [Required] public AttributeDataType DataType { get; set; } = default!;
        
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
        
        public int DefaultValueIndex { get; set; }
        public int DefaultUnitIndex { get; set; }

        public ICollection<string> Values { get; set; } = default!;
        public ICollection<string> Units { get; set; } = default!;
    }

    public class AttributeTypePatchDTO : DomainEntityId
    {
        [Required] public string Name { get; set; } = default!;
        
        public string? DefaultCustomValue { get; set; }
        
        [Required] public AttributeDataType DataType { get; set; } = default!;

        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }
    }
}