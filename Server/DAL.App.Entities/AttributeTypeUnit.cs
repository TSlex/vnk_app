﻿using System.Text.Json.Serialization;
using DAL.Base;
using DAL.Base.Entities;

namespace DAL.App.Entities
{
    public class AttributeTypeUnit: DomainEntityIdMetadata
    {
        public string Value { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }
    }
}