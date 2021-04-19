using System.Threading.Tasks;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork
{
    public class BaseRepo<TEntity, TDTO, TDbContext>: IBaseRepo
        where TEntity : class, IDomainEntityId, new()
        where TDTO : class, IDomainEntityId, new()
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;
        protected readonly IUniversalMapper Mapper;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepo(TDbContext dbContext, IUniversalMapper mapper)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
            Mapper = mapper;
        }
        
        public async Task<bool> ExistsAsync(long id)
        {
            return await DbSet.AnyAsync(o => o.Id == id);
        }

        protected TEntity MapToEntity(TDTO dto)
        {
            return Mapper.Map<TDTO, TEntity>(dto);
        }
        
        protected TDTO MapToDTO(TEntity entity)
        {
            return Mapper.Map<TEntity, TDTO>(entity);
        }
    }
}