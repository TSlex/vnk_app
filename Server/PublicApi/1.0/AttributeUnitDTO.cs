using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi.v1
{
    public class AttributeUnitGetDTO
    {
        public string Unit { get; set; } = default!;
    }

    public class AttributeUnitPostDTO
    {
        [Required] public string Unit { get; set; } = default!;
        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributeUnitPutDTO : DomainEntityId
    {
        [Required] public string Unit { get; set; } = default!;
    }
}