using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Base;
using DAL.Base.Entities;
using PublicApi._1._0.Enums;

namespace PublicApi._1._0
{
    #region GET

    public class TemplateGetDTO: DomainEntityId
    {
        public string Name { get; set; } = default!;

        public ICollection<TemplateAttributeGetDTO>? Attributes { get; set; }
    }

    public class TemplateAttributeGetDTO: DomainEntityId
    {
        public long AttributeId { get; set; }
        public long TypeId { get; set; }
        
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public AttributeDataType DataType { get; set; }
        public bool Featured { get; set; }
    }

    #endregion

    #region POST

    public class TemplatePostDTO
    {
        [Required] public string Name { get; set; } = default!;

        public ICollection<TemplateAttributePostDTO> Attributes { get; set; } = default!;
    }

    public class TemplateAttributePostDTO
    {
        public bool Featured { get; set; }
        [Required] public long AttributeId { get; set; } = default!;
    }

    #endregion
    
    #region PATCH

    public class TemplatePatchDTO: DomainEntityId
    {
        [Required] public string Name { get; set; } = default!;
    }

    public class TemplateAttributePatchDTO: DomainEntityId
    {
        public bool Featured { get; set; }
        [Required] public long AttributeId { get; set; } = default!;
    }

    #endregion
}