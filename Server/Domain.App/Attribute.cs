using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class Attribute : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public string Name { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}