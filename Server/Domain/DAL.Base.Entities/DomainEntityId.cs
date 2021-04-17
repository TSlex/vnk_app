using System;
using DAL.Contracts;

namespace DAL.Base.Entities
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