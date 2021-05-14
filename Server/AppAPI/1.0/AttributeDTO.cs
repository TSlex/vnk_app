using System.ComponentModel.DataAnnotations;
using AppAPI._1._0.Enums;
using DAL.Base.Entities;

namespace AppAPI._1._0
{
    public class AttributeGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        
        public long TypeId { get; set; }
        
        public AttributeDataType DataType { get; set; }
        
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
    }

    public class AttributeDetailsGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        
        public long TypeId { get; set; }
        
        public string? DefaultValue { get; set; }
        public string? DefaultUnit { get; set; }
        
        public long UsedCount { get; set; }
        
        public AttributeDataType DataType { get; set; }
    }

    public class AttributePostDTO
    {
        [Required] public string Name { get; set; } = default!;
        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributePatchDTO : DomainEntityId
    {
        [Required] public string Name { get; set; } = default!;
        public long? AttributeTypeId { get; set; }
    }
}