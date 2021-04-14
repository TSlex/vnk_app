using System.Text.Json.Serialization;
using Domain.Base;

namespace Domain
{
    public class TemplateAttribute : DomainEntityIdMetadata
    {
        public bool Featured { get; set; }
        
        public long TemplateId { get; set; } = default!;
        [JsonIgnore] public Template? Template { get; set; }

        public long AttributeId { get; set; } = default!;
        [JsonIgnore] public Attribute? Attribute { get; set; }
    }
}