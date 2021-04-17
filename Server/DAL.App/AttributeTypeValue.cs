using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class AttributeTypeValue : DomainEntityIdMetadata
    {
        public string Value { get; set; } = default!;

        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }
    }
}