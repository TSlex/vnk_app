using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class AttributeType : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        private ICollection<Attribute>? Attributes { get; set; }
        private ICollection<TypeValue>? TypeValues { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}