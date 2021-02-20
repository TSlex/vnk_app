using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class TypeValue: DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public long AttributeTypeId { get; set; } = default!;
        public AttributeType? AttributeType { get; set; }
        
        private ICollection<OrderAttribute>? OrderAttributes { get; set; }
        
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}