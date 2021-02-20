using Domain.Base;

namespace Domain
{
    public class TemplateAttribute : DomainEntityIdMetadata
    {
        public long TemplateId { get; set; } = default!;
        public Template? Template { get; set; }

        public long AttributeId { get; set; } = default!;
        public Attribute? Attribute { get; set; }
    }
}