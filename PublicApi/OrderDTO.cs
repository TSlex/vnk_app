using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicApi.v1
{
    #region GET

    public class OrderGetDTO
    {
        public bool Completed { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public ICollection<OrderAttributeGetDTO>? Attributes { get; set; }
    }

    public class OrderAttributeGetDTO
    {
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    #endregion

    #region POST

    public class OrderPostDTO
    {
        public bool Completed { get; set; }

        [Required] public DateTime ExecutionDateTime { get; set; }

        public ICollection<OrderAttributePostDTO>? Attributes { get; set; }
    }

    public class OrderAttributePostDTO
    {
        [Required] public long AttributeId { get; set; } = default!;
        [Required] public long TypeValueId { get; set; } = default!;
    }

    #endregion
}