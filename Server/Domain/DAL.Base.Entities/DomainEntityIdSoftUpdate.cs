using System;
using DAL.Contracts;

namespace DAL.Base.Entities
{
    public abstract class DomainEntityIdSoftUpdate: DomainEntityIdSoftUpdate<long>, IDomainEntityId
    {
        
    }
    
    public abstract class DomainEntityIdSoftUpdate<TKey> : IDomainEntityId<TKey>, IDomainEntityMetadata, IDomainEntitySoftUpdate
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; } = default!;
        public long? MasterId { get; set; }

        public virtual string? CreatedBy { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual string? ChangedBy { get; set; }
        public virtual DateTime ChangedAt { get; set; }

        public virtual string? DeletedBy { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
    }
}