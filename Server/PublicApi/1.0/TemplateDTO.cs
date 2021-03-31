using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicApi.v1
{
    #region GET

    public class TemplateGetDTO
    {
        public string Name { get; set; } = default!;

        public ICollection<TemplateAttributeGetDTO>? Attributes { get; set; }
    }

    public class TemplateAttributeGetDTO
    {
        public string Name { get; set; } = default!;
    }

    #endregion

    #region POST

    public class TemplatePostDTO
    {
        [Required] public string Name { get; set; } = default!;

        public ICollection<TemplateAttributeGetDTO>? TemplateAttributes { get; set; }
    }

    public class TemplateAttributePostDTO
    {
        [Required] public string Name { get; set; } = default!;

        [Required] public long TemplateId { get; set; } = default!;
        [Required] public long AttributeId { get; set; } = default!;
    }

    #endregion
}