using System.Collections.Generic;
using DAL.Base;

namespace DAL.App
{
    public class Template : DomainEntityIdMetadata
    {
        public string Name { get; set; } = default!;
        
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}