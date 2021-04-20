using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork
{
    public class BaseRepo<TEntity, TDTO, TDbContext> : IBaseRepo<TEntity, TDTO>
        where TEntity : class, IDomainEntityId, new()
        where TDTO : class, IDomainEntityId, new()
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;
        protected readonly IUniversalMapper Mapper;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepo(TDbContext dbContext, IUniversalMapper mapper)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
            Mapper = mapper;
        }

        public virtual async Task<Func<long>> AddAsync(TDTO dto)
        {
            var entity = await DbSet.AddAsync(MapToEntity(dto));

            return () => entity.Entity.Id;
        }

        public virtual async Task<bool> AnyAsync(long id)
        {
            var query = GetActualDataByIdAsQueryable(id);

            return await query.AnyAsync();
        }

        public virtual async Task<bool> AnyIncludeDeletedAsync(long id)
        {
            var query = DbSet.Where(e => e.Id == id);

            if (IsEntitySoftUpdatable())
            {
                query = query.Where(e => (e as IDomainEntitySoftUpdate)!.MasterId == null);
            }

            return await query.AnyAsync();
        }

        public virtual async Task<TDTO> FirstOrDefaultAsync(long id)
        {
            var query = GetActualDataByIdAsQueryable(id);

            return MapToDTO(await query.FirstOrDefaultAsync());
        }

        public virtual async Task UpdateAsync(TDTO dto)
        {
            TEntity trackedEntity = await DbSet.FindAsync(dto.Id);
            TEntity entityToTrack = MapToEntity(dto);

            DbContext.Entry(trackedEntity).State = EntityState.Detached;

            if (trackedEntity is IDomainEntitySoftUpdate softUpdate)
            {
                softUpdate.MasterId = trackedEntity.Id;
                trackedEntity.Id = 0;

                await DbSet.AddAsync(trackedEntity);
            }

            DbSet.Update(entityToTrack);
        }


        public virtual async Task RemoveAsync(TDTO dto)
        {
            await RemoveAsync(dto.Id);
        }

        public virtual async Task RemoveAsync(long id)
        {
            TEntity trackedEntity = await DbSet.FindAsync(id);

            DbSet.Remove(trackedEntity);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<TDTO> enumerable)
        {
            foreach (var dto in enumerable)
            {
                await RemoveAsync(dto);
            }
        }

        public virtual async Task RestoreAsync(long id)
        {
            var deletedEntity = await DbSet.FindAsync(id);

            if (deletedEntity is IDomainEntitySoftDelete softDelete)
            {
                softDelete.DeletedAt = null;
                softDelete.DeletedBy = null;
            }

            DbSet.Update(deletedEntity);
        }

        protected TEntity MapToEntity(TDTO dto)
        {
            return Mapper.Map<TDTO, TEntity>(dto);
        }

        protected TDTO MapToDTO(TEntity entity)
        {
            return Mapper.Map<TEntity, TDTO>(entity);
        }

        protected bool IsEntitySoftDeletable()
        {
            return typeof(TEntity).GetInterfaces().Contains(typeof(IDomainEntitySoftDelete));
        }

        protected bool IsEntitySoftUpdatable()
        {
            return typeof(TEntity).GetInterfaces().Contains(typeof(IDomainEntitySoftUpdate));
        }

        protected IQueryable<TEntity> GetActualDataByIdAsQueryable(long id)
        {
            var query = DbSet.Where(e => e.Id == id);

            if (IsEntitySoftDeletable())
            {
                query = query.Where(e => (e as IDomainEntitySoftDelete)!.DeletedAt == null);
            }

            if (IsEntitySoftUpdatable())
            {
                query = query.Where(e => (e as IDomainEntitySoftUpdate)!.MasterId == null);
            }

            return query;
        }

        protected IQueryable<TEntity> GetActualDataAsQueryable()
        {
            var query = DbSet.AsQueryable();

            if (IsEntitySoftDeletable())
            {
                query = query.Where(e => (e as IDomainEntitySoftUpdate)!.DeletedAt == null);
            }

            if (IsEntitySoftUpdatable())
            {
                query = query.Where(e => (e as IDomainEntitySoftUpdate)!.MasterId == null);
            }

            return query;
        }
    }
}