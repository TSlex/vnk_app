﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork
{
    public class BaseRepo<TEntity, TDTO, TDbContext>: IBaseRepo<TEntity, TDTO>
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
        
        public virtual async Task<bool> AnyAsync(long id)
        {
            return await DbSet.AnyAsync(o => o.Id == id);
        }
        
        public virtual async Task UpdateAsync(TDTO dto)
        {
            var now = DateTime.UtcNow;
            
            TEntity trackedEntity = await DbSet.FindAsync(dto.Id);
            TEntity entityToTrack = MapToEntity(dto);

            DbContext.Entry(trackedEntity).State = EntityState.Detached;

            if (trackedEntity is IDomainEntitySoftUpdate softUpdate)
            {
                softUpdate.MasterId = trackedEntity.Id;
                softUpdate.DeletedAt = now;
                softUpdate.DeletedBy = "history";
                trackedEntity.Id = 0;

                await DbSet.AddAsync(trackedEntity);
            }
                
            DbSet.Update(entityToTrack);
        }

        public virtual async Task RemoveAsync(TDTO dto)
        {
            TEntity trackedEntity = await DbSet.FindAsync(dto.Id);
            // TEntity entityToTrack = MapToEntity(dto);
            //
            // DbContext.Entry(trackedEntity).State = EntityState.Detached;
            // trackedEntity.Id = 0;
            
            DbSet.Remove(trackedEntity);
        }
        
        public virtual async Task RemoveRangeAsync(IEnumerable<TDTO> enumerable)
        {
            foreach (var dto in enumerable)
            {
                await RemoveAsync(dto);
            }
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