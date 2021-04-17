using System.ComponentModel.DataAnnotations;
using DAL.Base;
using DAL.Base.Entities;

namespace PublicApi._1._0
{
    public class AttributeTypeValueGetDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeTypeValuePostDTO
    {
        [Required] public string Value { get; set; } = default!;
        [Required] public long AttributeTypeId { get; set; } = default!;
    }

    public class AttributeTypeValuePatchDTO : DomainEntityId
    {
        [Required] public string Value { get; set; } = default!;
    }
}