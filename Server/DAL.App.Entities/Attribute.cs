using System.Collections.Generic;
using System.Text.Json.Serialization;
using DAL.Base;
using DAL.Base.Entities;

namespace DAL.App.Entities
{
    public class Attribute : DomainEntityIdSoftDelete
    {
        public string Name { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}