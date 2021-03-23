using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class Template : DomainEntityIdMetadata
    {
        public string Name { get; set; } = default!;
        
        public ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}