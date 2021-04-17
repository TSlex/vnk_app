using System;
using System.Collections.Generic;
using DAL.Base;

namespace DAL.App
{
    public class Order: DomainEntityIdSoftDelete
    {
        public string Name { get; set; } = default!;
        
        public bool Completed { get; set; }
        public string? Notation { get; set; }

        public DateTime? ExecutionDateTime { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }
    }
}