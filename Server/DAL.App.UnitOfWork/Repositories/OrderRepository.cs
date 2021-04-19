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
    public class OrderRepository : BaseRepo<AppDbContext>, IOrderRepository
    {
        public OrderRepository(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
        
        public async Task<IEnumerable<Order>> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            var ordersQuery = DbContext.Orders
                .WhereActual()
                .IncludeAttributesFull()
                .AsQueryable();

            ordersQuery = ordersQuery.WhereSuid(hasExecutionDate, completed, searchKey, startDateTime,
                endDateTime);

            ordersQuery = ordersQuery.OrderBy(at => at.Id);

            ordersQuery = byName switch
            {
                SortOption.True => ordersQuery.OrderBy(at => at.Name),
                SortOption.Reversed => ordersQuery.OrderByDescending(at => at.Name),
                _ => ordersQuery
            };

            ordersQuery = ordersQuery.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            return (await ordersQuery.ToListAsync()).Select(Mapper.Map<Entities.Order, Order>);
        }

        public async Task<int> Count(bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            var ordersQuery = DbContext.Orders.WhereActual();

            return await ordersQuery.WhereSuid(hasExecutionDate, completed, searchKey, startDateTime,
                endDateTime).CountAsync();
        }
    }

    internal static class Extensions
    {
        internal static IQueryable<Entities.Order> WhereSuid(this IQueryable<Entities.Order> query,
            bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            query = hasExecutionDate
                ? query.Where(o => o.ExecutionDateTime != null)
                : query.Where(o => o.ExecutionDateTime == null);

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