using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi.v1
{
    public class AttributeTypeUnitGetDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeTypeUnitPostDTO
    {
        [Required] public string Value { get; set; } = default!;
        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributeTypeUnitPatchDTO : DomainEntityId
    {
        [Required] public string Value { get; set; } = default!;
    }
}