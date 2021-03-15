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
        
        public AttributeDataType DataType { get; set; } = default!;
        
        public ICollection<Attribute>? Attributes { get; set; }
        public ICollection<TypeValue>? TypeValues { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}