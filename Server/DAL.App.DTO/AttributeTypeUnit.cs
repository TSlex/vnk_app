using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class AttributeTypeUnit: DomainEntityIdSoftDelete
    {
        public string Value { get; set; } = default!;
        
        public long AttributeTypeId { get; set; } = default!;
        public AttributeType? AttributeType { get; set; }
    }
}