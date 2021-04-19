using System.Collections.Generic;
using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class Template : DomainEntityIdMetadata
    {
        public string Name { get; set; } = default!;
        
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}