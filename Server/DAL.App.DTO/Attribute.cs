using System.Collections.Generic;
using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class Attribute : DomainEntityIdSoftUpdate
    {
        public string Name { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        public AttributeType? AttributeType { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}