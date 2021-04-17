using System;

namespace DAL.Contracts
{
    public interface IDomainEntitySoftDelete
    {
        string? DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}