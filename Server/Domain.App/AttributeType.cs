using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;
using Domain.Enums;

namespace Domain
{
    public class AttributeType : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public string Name { get; set; } = default!;
        
        public AttributeDataType DataType { get; set; }
        
        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }
        
        public ICollection<Attribute>? Attributes { get; set; }
        public ICollection<AttributeTypeValue>? TypeValues { get; set; }
        public ICollection<AttributeTypeUnit>? TypeUnits { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}