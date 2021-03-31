using System;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class AttributeTypeUnit: DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public string Value { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }
        
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}