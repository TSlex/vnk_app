using System;
using System.Collections.Generic;
using DAL.Base.Entities;

namespace DAL.App.DTO
{
    public class Order: DomainEntityIdSoftUpdate
    {
        public string Name { get; set; } = default!;
        
        public bool Completed { get; set; }
        public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }
    }
}