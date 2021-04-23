using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class OrderAttribute : DomainEntityIdSoftUpdate
    {
        public bool Featured { get; set; }
        
        public string? CustomValue { get; set; }
        
        public long? ValueId { get; set; }
        public AttributeTypeValue? Value { get; set; }
        
        public long? UnitId { get; set; }
        public AttributeTypeUnit? Unit { get; set; }

        public long OrderId { get; set; } = default!;
        public Order? Order { get; set; }

        public long AttributeId { get; set; } = default!;
        public Attribute? Attribute { get; set; }
    }
}