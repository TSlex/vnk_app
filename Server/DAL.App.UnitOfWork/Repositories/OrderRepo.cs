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
            SortOption byName, bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey,
            DateTime? startDateTime, DateTime? endDateTime, DateTime? checkDateTime)
        {
            var query = GetActualDataAsQueryable()
                .IncludeAttributesFull()
                .AsQueryable();

            query = query.WhereSuidConditions(hasExecutionDate, completed, overdued, searchKey, startDateTime,
                endDateTime, checkDateTime);

            query = query.OrderBy(at => at.ExecutionDateTime).ThenBy(at => at.Id);

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
            var query = GetActualDataAsQueryable()
                .IncludeAttributesFull()
                .Where(o => o.Id == id);

            return Mapper.Map<Entities.Order, Order>(await query.FirstOrDefaultAsync());
        }

        public async Task<int> CountAsync(bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey,
            DateTime? startDateTime, DateTime? endDateTime, DateTime? checkDateTime)
        {
            var ordersQuery = GetActualDataAsQueryable();

            return await ordersQuery.WhereSuidConditions(hasExecutionDate, completed, overdued, searchKey,
                startDateTime,
                endDateTime, checkDateTime).CountAsync();
        }

        #region History

        public async Task<IEnumerable<Order>> GetHistoryAsync(long id, int pageIndex, int itemsOnPage)
        {
            var query = DbSet.Where(o => o.Id == id || o.MasterId == id);

            query = query.OrderByDescending(at => at.ChangedAt);

            var orders = await query.Skip(pageIndex * itemsOnPage).Take(itemsOnPage).ToListAsync();

            var orderAttributes = await DbContext.OrderAttributes
                // .Include(oa => oa.Attribute)
                // .ThenInclude(a => a!.AttributeType)
                // .ThenInclude(t => t!.TypeValues)
                // .Include(oa => oa.Attribute)
                // .ThenInclude(a => a!.AttributeType)
                // .ThenInclude(t => t!.TypeUnits)
                .Where(oa => oa.OrderId == id).ToListAsync();

            var orderAttributesId = orderAttributes.Select(oa => oa.AttributeId);
            var attributes = await DbContext.Attributes.Where(a => orderAttributesId.Contains(a.Id)).ToListAsync();

            var attributeTypesId = attributes.Select(a => a.AttributeTypeId);
            var types = await DbContext.AttributeTypes
                .Where(t => attributeTypesId.Contains(t.Id))
                .Include(t => t.TypeValues)
                .Include(t => t.TypeUnits)
                .ToListAsync();


            foreach (var order in orders)
            {
                order.OrderAttributes = orderAttributes
                    .Where(oa =>
                        oa.ChangedAt <= order.ChangedAt &&
                        (!oa.DeletedAt.HasValue || oa.DeletedAt >= order.ChangedAt || oa.DeletedAt >= order.DeletedAt)
                    )
                    .ToList();

                foreach (var orderAttribute in order.OrderAttributes)
                {
                    orderAttribute.Attribute = attributes
                        .FirstOrDefault(a =>
                            a.ChangedAt <= orderAttribute.ChangedAt &&
                            (!a.DeletedAt.HasValue || a.DeletedAt >= orderAttribute.ChangedAt ||
                             a.DeletedAt >= orderAttribute.DeletedAt));

                    if (orderAttribute.Attribute != null)
                    {
                        orderAttribute.Attribute.AttributeType = types
                            .FirstOrDefault(t =>
                                t.ChangedAt <= orderAttribute.Attribute.ChangedAt &&
                                (!t.DeletedAt.HasValue || t.DeletedAt >= orderAttribute.Attribute.ChangedAt ||
                                 t.DeletedAt >= orderAttribute.Attribute.DeletedAt));
                    }
                }
            }

            return orders.Select(MapToDTO);
        }

        public async Task<long> CountHistoryAsync(long id)
        {
            return await DbSet.CountAsync(o => o.Id == id || o.MasterId == id);
        }

        #endregion
    }

    internal static partial class Extensions
    {
        internal static IQueryable<Entities.Order> WhereSuidConditions(this IQueryable<Entities.Order> query,
            bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDateTime)
        {
            if (hasExecutionDate != null)
            {
                query = hasExecutionDate.Value
                    ? query.Where(o => o.ExecutionDateTime != null)
                    : query.Where(o => o.ExecutionDateTime == null);
            }

            if (startDateTime != null)
            {
                query = query.Where(o => o.ExecutionDateTime.HasValue && o.ExecutionDateTime >= startDateTime);
            }

            if (endDateTime != null)
            {
                query = query.Where(o => o.ExecutionDateTime.HasValue && o.ExecutionDateTime < endDateTime);
            }

            if (overdued != null)
            {
                checkDateTime ??= DateTime.Now;

                query = overdued.Value
                    ? query.Where(o => o.ExecutionDateTime.HasValue && o.ExecutionDateTime < checkDateTime)
                    : query.Where(o => o.ExecutionDateTime.HasValue && o.ExecutionDateTime >= checkDateTime);
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

        internal static IQueryable<Entities.Order> IncludeAttributesFull(this IQueryable<Entities.Order> query)
        {
            return query
                .Include(o => o.OrderAttributes!.Where(oa =>
                    oa.DeletedAt == null))
                .ThenInclude(oa => oa.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .ThenInclude(t => t!.TypeValues!.Where(v =>
                    v.DeletedAt == null))
                .Include(o => o.OrderAttributes!.Where(oa =>
                    oa.DeletedAt == null))
                .ThenInclude(oa => oa.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .ThenInclude(t => t!.TypeUnits!.Where(u =>
                    u.DeletedAt == null));
        }
    }
}