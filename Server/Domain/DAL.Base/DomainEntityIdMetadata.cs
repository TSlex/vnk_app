using System;
using DAL.Contracts;

namespace DAL.Base
{
    public abstract class DomainEntityIdMetadata : DomainEntityIdMetadata<long>
    {
    }

    public abstract class DomainEntityIdMetadata<TKey> : IDomainEntityId<TKey>, IDomainEntityMetadata
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; } = default!;

        public virtual string? CreatedBy { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual string? ChangedBy { get; set; }
        public virtual DateTime ChangedAt { get; set; }
    }
}