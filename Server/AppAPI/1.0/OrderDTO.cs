using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppAPI._1._0.Enums;
using DAL.Base.Entities;

namespace AppAPI._1._0
{
    #region GET

    public class OrderGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;

        public bool Completed { get; set; }
        public bool Overdued { get; set; }

        public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        public ICollection<OrderAttributeGetDTO>? Attributes { get; set; }
    }

    public class OrderHistoryGetDTO : DomainEntityIdSoftUpdate
    {
        public string Name { get; set; } = default!;

        public bool Completed { get; set; }
        public bool Overdued { get; set; }

        public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        public ICollection<OrderAttributeGetDTO>? Attributes { get; set; }
    }

    public class OrderAttributeGetDTO : DomainEntityId
    {
        public string Name { get; set; } = default!;

        public bool UsesDefinedValues { get; set; }
        public bool UsesDefinedUnits { get; set; }

        public string? Value { get; set; }
        public string? Unit { get; set; }

        public long? ValueId { get; set; }
        public long? UnitId { get; set; }

        public long AttributeId { get; set; }
        public long TypeId { get; set; }

        public string Type { get; set; } = default!;
        public AttributeDataType DataType { get; set; }

        public bool Featured { get; set; }
        
        public long? MasterId { get; set; }
    }

    #endregion

    #region POST

    public class OrderPostDTO
    {
        [Required] [MaxLength(100)] public string Name { get; set; } = default!;

        public bool Completed { get; set; }

        [MaxLength(1000)] public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        [MaxLength(30)] public ICollection<OrderAttributePostDTO> Attributes { get; set; } = default!;
    }

    public class OrderAttributePostDTO
    {
        public bool Featured { get; set; }

        [Required] public long AttributeId { get; set; } = default!;

        [MaxLength(200)] public string? CustomValue { get; set; }
        public long? ValueId { get; set; }
        public long? UnitId { get; set; }
    }

    #endregion

    #region PATCH

    public class OrderPatchDTO : DomainEntityId
    {
        [Required] [MaxLength(100)] public string Name { get; set; } = default!;

        public bool Completed { get; set; }

        [MaxLength(1000)] public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        [MaxLength(30)] public ICollection<OrderAttributePatchDTO> Attributes { get; set; } = default!;
    }

    public class OrderAttributePatchDTO
    {
        public long? Id { get; set; }

        public PatchOption PatchOption { get; set; }

        public bool Featured { get; set; }

        [Required] public long AttributeId { get; set; } = default!;

        [MaxLength(200)] public string? CustomValue { get; set; }
        public long? ValueId { get; set; }
        public long? UnitId { get; set; }
    }

    public class OrderCompletionPatchDTO : DomainEntityId
    {
        public bool Completed { get; set; }
    }

    #endregion
}