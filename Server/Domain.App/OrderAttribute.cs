﻿using System;
using System.Text.Json.Serialization;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class OrderAttribute : DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        //has to be displayed in calendar view
        public bool Featured { get; set; }
        
        //custom value if not selected from list
        public string? CustomValue { get; set; }
        
        //attribute value if selected from list
        public long? ValueId { get; set; }
        [JsonIgnore] public AttributeTypeValue? Value { get; set; }
        
        //attribute unit if composite
        public long? UnitId { get; set; }
        [JsonIgnore] public AttributeTypeUnit? Unit { get; set; }

        //order to belong
        public long OrderId { get; set; } = default!;
        [JsonIgnore] public Order? Order { get; set; }

        //label
        public long AttributeId { get; set; } = default!;
        [JsonIgnore] public Attribute? Attribute { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}