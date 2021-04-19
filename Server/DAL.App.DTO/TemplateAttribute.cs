using System.Text.Json.Serialization;
using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class TemplateAttribute : DomainEntityIdMetadata
    {
        public bool Featured { get; set; }
        
        public long TemplateId { get; set; } = default!;
        public Template? Template { get; set; }

        public long AttributeId { get; set; } = default!;
        public Attribute? Attribute { get; set; }
    }
}