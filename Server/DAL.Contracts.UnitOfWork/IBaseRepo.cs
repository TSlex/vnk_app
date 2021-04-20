using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseRepo<TEntity, TDTO>
        where TEntity : class, IDomainEntityId, new()
        where TDTO : class, IDomainEntityId, new()
    {
        Task<Func<long>> AddAsync(TDTO dto);
        
        Task<bool> AnyAsync(long id);
        Task<bool> AnyIncludeDeletedAsync(long id);
        
        Task<TDTO> FirstOrDefaultAsync(long id);
        
        Task UpdateAsync(TDTO dto);
        
        Task RemoveAsync(TDTO dto);
        Task RemoveAsync(long id);
        
        Task RemoveRangeAsync(IEnumerable<TDTO> enumerable);
        
        Task RestoreAsync(long id);
    }
}