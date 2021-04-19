using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class OrderRepo : BaseRepo<Entities.Order, Order, AppDbContext>, IOrderRepo
    {
        public OrderRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, bool? hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            var query = DbSet
                .WhereActual()
                .IncludeAttributesFull()
                .AsQueryable();

            query = query.WhereSuid(hasExecutionDate, completed, searchKey, startDateTime,
                endDateTime);

            query = query.OrderBy(at => at.Id);

            query = byName switch
            {
                SortOption.True => query.OrderBy(at => at.Name),
                SortOption.Reversed => query.OrderByDescending(at => at.Name),
                _ => query
            };

            query = query.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            return (await query.ToListAsync()).Select(Mapper.Map<Entities.Order, Order>);
        }

        public async Task<Order> GetByIdAsync(long id)
        {
            var query = DbSet
                .WhereActual()
                .IncludeAttributesFull()
                .Where(o => o.Id == id);

            return Mapper.Map<Entities.Order, Order>(await query.FirstOrDefaultAsync());
        }

        public async Task AddAsync(Order order)
        {
            order.Id = (await DbSet.AddAsync(MapToEntity(order))).Entity.Id;
        }

        public async Task<Order> FirstOrDefaultAsync(long id)
        {
            return MapToDTO(await DbSet.FirstOrDefaultAsync(order => order.Id == id));
        }

        public void Update(Order order)
        {
            DbSet.Update(MapToEntity(order));
        }

        public void Remove(Order order)
        {
            DbSet.Remove(MapToEntity(order));
        }

        public async Task<int> CountAsync(bool? hasExecutionDate, bool? completed, string? searchKey,
            DateTime? startDateTime,
            DateTime? endDateTime)
        {
            var ordersQuery = DbSet.WhereActual();

            return await ordersQuery.WhereSuid(hasExecutionDate, completed, searchKey, startDateTime,
                endDateTime).CountAsync();
        }
    }

    internal static class Extensions
    {
        internal static IQueryable<Entities.Order> WhereSuid(this IQueryable<Entities.Order> query,
            bool? hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            if (hasExecutionDate != null)
            {
                query = hasExecutionDate.Value
                    ? query.Where(o => o.ExecutionDateTime != null)
                    : query.Where(o => o.ExecutionDateTime == null);
            }

            if (startDateTime != null)
            {
                query = query.Where(o => o.ExecutionDateTime >= startDateTime);
            }

            if (endDateTime != null)
            {
                query = query.Where(o => o.ExecutionDateTime < endDateTime);
            }

            if (completed != null)
            {
                query = completed.Value
                    ? query.Where(o => o.Completed)
                    : query.Where(o => !o.Completed);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(a => a.Name.ToLower().Contains(searchKey.ToLower()));
            }

            return query.Where(o => o.DeletedAt == null && o.MasterId == null);
        }

        internal static IQueryable<Entities.Order> WhereActual(this IQueryable<Entities.Order> query)
        {
            return query.Where(o => o.DeletedAt == null && o.MasterId == null);
        }

        internal static IQueryable<Entities.Order> IncludeAttributesFull(this IQueryable<Entities.Order> query)
        {
            return query
                .Include(o => o.OrderAttributes)
                .ThenInclude(oa => oa.Attribute)
                .ThenInclude(a => a!.AttributeType);
        }
    }
}