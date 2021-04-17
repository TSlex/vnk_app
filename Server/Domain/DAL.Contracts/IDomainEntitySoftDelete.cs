using System;

namespace Contracts.Domain
{
    public interface IDomainEntitySoftDelete
    {
        string? DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}