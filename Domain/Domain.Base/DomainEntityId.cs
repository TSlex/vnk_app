using System;
using Contracts.Domain;

namespace Domain.Base
{
    public abstract class DomainEntityId : IDomainEntityId
    {
        public virtual long Id { get; set; }
    }

    public abstract class DomainEntityId<TKey> : IDomainEntityId<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; } = default!;
    }
}