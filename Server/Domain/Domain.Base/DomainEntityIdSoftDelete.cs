using System;
using Contracts.Domain;

namespace Domain.Base
{
    public class DomainEntityIdSoftDelete: DomainEntityIdSoftDelete<long>
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