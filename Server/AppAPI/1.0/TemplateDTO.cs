using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppAPI._1._0.Enums;
using DAL.Base.Entities;

namespace AppAPI._1._0
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
        
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
    }

    #endregion

    #region POST

    public class TemplatePostDTO
    {
        [Required][MaxLength(100)]  public string Name { get; set; } = default!;

        [MaxLength(30)] public ICollection<TemplateAttributePostDTO> Attributes { get; set; } = default!;
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
        [Required][MaxLength(100)]  public string Name { get; set; } = default!;
        
        [MaxLength(30)] public ICollection<TemplateAttributePatchDTO> Attributes { get; set; } = default!;
    }

    public class TemplateAttributePatchDTO
    {
        public long? Id { get; set; }
        
        public PatchOption PatchOption { get; set; }
        
        public bool Featured { get; set; }
        
        [Required] public long AttributeId { get; set; } = default!;
    }

    #endregion
}