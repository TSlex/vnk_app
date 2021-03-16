using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace PublicApi
{
    public class AttributeValueGetDTO
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeValuePostDTO
    {
        [Required] public string Value { get; set; } = default!;
        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributeValuePutDTO : DomainEntityId
    {
        [Required] public string Value { get; set; } = default!;
    }
}