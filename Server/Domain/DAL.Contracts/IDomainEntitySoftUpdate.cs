using System;

namespace Contracts.Domain
{
    public interface IDomainEntitySoftUpdate: IDomainEntitySoftUpdate<long>
    {
    }
    
    public interface IDomainEntitySoftUpdate<TKey> : IDomainEntitySoftDelete
    where TKey : struct, IEquatable<TKey>
    {
        TKey? MasterId { get; set; }
    }
}