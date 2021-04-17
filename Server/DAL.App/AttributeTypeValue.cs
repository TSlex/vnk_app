using System.Text.Json.Serialization;
using DAL.Base;

namespace DAL.App
{
    public class AttributeTypeValue : DomainEntityIdMetadata
    {
        public string Value { get; set; } = default!;

        public long AttributeTypeId { get; set; } = default!;
        [JsonIgnore] public AttributeType? AttributeType { get; set; }
    }
}