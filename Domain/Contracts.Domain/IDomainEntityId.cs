using System;

namespace Contracts.Domain
{
    public interface IDomainEntityId : IDomainEntityId<long>
    {
    }

    public interface IDomainEntityId<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}