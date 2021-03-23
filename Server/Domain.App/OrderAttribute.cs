using System;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class OrderAttribute : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public long OrderId { get; set; } = default!;
        [JsonIgnore] public Order? Order { get; set; }

        public long AttributeId { get; set; } = default!;
        [JsonIgnore] public Attribute? Attribute { get; set; }

        public long AttributeTypeValueId { get; set; } = default!;
        [JsonIgnore] public AttributeTypeValue? AttributeTypeValue { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}