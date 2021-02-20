using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class Template : DomainEntityIdMetadata
    {
        private ICollection<TemplateAttribute>? TemplateAttributes { get; set; }
    }
}