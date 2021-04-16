
using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
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