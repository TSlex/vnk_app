﻿
using System;
using System.Collections.Generic;
using Contracts.Domain;
using Domain.Base;

namespace Domain
{
    public class Order: DomainEntityIdMetadata, IDomainEntitySoftDelete
    {
        public bool Completed { get; set; }
        
        public DateTime ExecutionDateTime { get; set; }

        public ICollection<OrderAttribute>? OrderAttributes { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}