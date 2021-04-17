using System.Text.Json.Serialization;
using DAL.Base;
using DAL.Base.Entities;

namespace DAL.App.Entities
{
    public class OrderAttribute : DomainEntityIdSoftDelete
    {
        //has to be displayed in calendar view
        public bool Featured { get; set; }
        
        //custom value if not selected from list
        public string? CustomValue { get; set; }
        
        //attribute value if selected from list
        public long? ValueId { get; set; }
        [JsonIgnore] public AttributeTypeValue? Value { get; set; }
        
        //attribute unit if composite
        public long? UnitId { get; set; }
        [JsonIgnore] public AttributeTypeUnit? Unit { get; set; }

        //order to belong
        public long OrderId { get; set; } = default!;
        [JsonIgnore] public Order? Order { get; set; }

        //label
        public long AttributeId { get; set; } = default!;
        [JsonIgnore] public Attribute? Attribute { get; set; }
    }
}