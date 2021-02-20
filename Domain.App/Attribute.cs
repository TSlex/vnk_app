using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class Attribute : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public bool Displayed { get; set; }

        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }

        private ICollection<OrderAttribute>? OrderAttributes { get; set; }
        private ICollection<TemplateAttribute>? TemplateAttributes { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}