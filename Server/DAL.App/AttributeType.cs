using System.Collections.Generic;
using DAL.App.Enums;
using DAL.Base;

namespace DAL.App
{
    public class AttributeType : DomainEntityIdSoftDelete
    {
        public string Name { get; set; } = default!;
        public string? DefaultCustomValue { get; set; }

        public bool SystemicType { get; set; }
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }

        public AttributeDataType DataType { get; set; }

        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }

        public ICollection<Attribute>? Attributes { get; set; }
        public ICollection<AttributeTypeValue>? TypeValues { get; set; }
        public ICollection<AttributeTypeUnit>? TypeUnits { get; set; }
    }
}