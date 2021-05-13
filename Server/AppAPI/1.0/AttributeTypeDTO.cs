using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppAPI._1._0.Enums;
using DAL.Base.Entities;

namespace AppAPI._1._0
{
    #region GET

    public class AttributeTypeGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public long UsedCount { get; set; }

        public AttributeDataType DataType { get; set; } = default!;

        public bool SystemicType { get; set; }
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }
    }

    public class AttributeTypeDetailsGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;
        public long UsedCount { get; set; }

        public string DefaultCustomValue { get; set; } = default!;

        public AttributeDataType DataType { get; set; } = default!;

        public bool SystemicType { get; set; }
        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }

        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }

        public long ValuesCount { get; set; }
        public long UnitsCount { get; set; }

        public ICollection<AttributeTypeValueDetailsDTO> Values { get; set; } = default!;
        public ICollection<AttributeTypeUnitDetailsDTO> Units { get; set; } = default!;
    }

    public class AttributeTypeValueDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    public class AttributeTypeUnitDetailsDTO : DomainEntityId
    {
        public string Value { get; set; } = default!;
    }

    #endregion

    #region POST

    public class AttributeTypePostDTO
    {
        [Required] [MaxLength(100)] public string Name { get; set; } = default!;

        [MaxLength(200)] public string? DefaultCustomValue { get; set; }

        [Required] public AttributeDataType DataType { get; set; } = default!;

        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }

        public int DefaultValueIndex { get; set; }
        public int DefaultUnitIndex { get; set; }

        [MaxLength(100)] public ICollection<string> Values { get; set; } = default!;
        [MaxLength(100)] public ICollection<string> Units { get; set; } = default!;
    }

    #endregion

    #region PATCH

    public class AttributeTypePatchDTO : DomainEntityId
    {
        [Required] [MaxLength(100)] public string Name { get; set; } = default!;

        [MaxLength(200)] public string? DefaultCustomValue { get; set; }

        [Required] public AttributeDataType DataType { get; set; } = default!;

        public long DefaultValueId { get; set; }
        public long DefaultUnitId { get; set; }

        [MaxLength(100)] public ICollection<AttributeTypeValuePatchDTO> Values { get; set; } = default!;
        [MaxLength(100)] public ICollection<AttributeTypeUnitPatchDTO> Units { get; set; } = default!;
    }

    public class AttributeTypeValuePatchDTO
    {
        [Required] [MaxLength(200)] public string Value { get; set; } = default!;
        public PatchOption PatchOption { get; set; }
        public long? Id { get; set; }
    }

    public class AttributeTypeUnitPatchDTO
    {
        [Required] [MaxLength(200)] public string Value { get; set; } = default!;
        public PatchOption PatchOption { get; set; }
        public long? Id { get; set; }
    }

    #endregion
}