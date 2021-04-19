using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseRepo<TEntity, TDTO>
        where TEntity : class, IDomainEntityId, new()
        where TDTO : class, IDomainEntityId, new()
    {
        Task<bool> AnyAsync(long id);
        Task UpdateAsync(TDTO dto);
        Task RemoveAsync(TDTO dto);
        Task RemoveRangeAsync(IEnumerable<TDTO> enumerable);
        
    }
}