using System;
using DAL.Contracts;

namespace DAL.Base.Entities
{
    public abstract class DomainEntityIdSoftDelete: DomainEntityIdSoftDelete<long>, IDomainEntityId
    {
        
    }
    
    public abstract class DomainEntityIdSoftDelete<TKey> : IDomainEntityId<TKey>, IDomainEntityMetadata, IDomainEntitySoftDelete
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; } = default!;

        public virtual string? CreatedBy { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual string? ChangedBy { get; set; }
        public virtual DateTime ChangedAt { get; set; }
        
        public virtual string? DeletedBy { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
    }
}