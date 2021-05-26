using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            var orders = await query.Select(ProjectOrder()).ToListAsync();

            return orders.Select(Mapper.Map<Entities.Order, Order>);
        }

        public async Task<Order> GetByIdAsync(long id)
        {
            var query = GetActualDataByIdAsQueryable(id);

            var order = await query.Select(ProjectOrder()).FirstOrDefaultAsync();

            return Mapper.Map<Entities.Order, Order>(order);
        }

        public async Task<int> CountAsync(bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey,
            DateTime? startDateTime, DateTime? endDateTime, DateTime? checkDateTime)
        {
            var ordersQuery = GetActualDataAsQueryable();

            return await ordersQuery.WhereSuidConditions(hasExecutionDate, completed, overdued, searchKey,
                startDateTime, endDateTime, checkDateTime).CountAsync();
        }

        #region History

        public async Task<IEnumerable<Order>> GetHistoryAsync(long id, int pageIndex, int itemsOnPage)
        {
            var query = DbSet.Where(o => o.Id == id || o.MasterId == id);

            query = query.OrderByDescending(at => at.ChangedAt);

            var orders = await query.Skip(pageIndex * itemsOnPage).Take(itemsOnPage).ToListAsync();

            var orderAttributes = await DbContext.OrderAttributes
                .Include(oa => oa.Value)
                .Include(oa => oa.Unit)
                .Where(oa => oa.OrderId == id)
                .ToListAsync();

            var attributesId = orderAttributes.Select(oa => oa.AttributeId).ToList();
            var attributes = await DbContext.Attributes
                .Where(a =>
                    attributesId.Contains(a.Id) ||
                    a.MasterId.HasValue && attributesId.Contains(a.MasterId.Value))
                .ToListAsync();

            var typesId = attributes.Select(a => a.AttributeTypeId).ToList();
            var types = await DbContext.AttributeTypes
                .Where(t =>
                    typesId.Contains(t.Id) ||
                    t.MasterId.HasValue && typesId.Contains(t.MasterId.Value)
                )
                .ToListAsync();

            foreach (var order in orders)
            {
                order.OrderAttributes = orderAttributes
                    .Where(oa =>
                        !(oa.ChangedAt > order.ChangedAt ||
                          oa.DeletedAt <= order.ChangedAt)
                    )
                    .Select(Mapper.Map<Entities.OrderAttribute, Entities.OrderAttribute>)
                    .ToList();

                foreach (var orderAttribute in order.OrderAttributes)
                {
                    orderAttribute.Attribute = attributes
                        .OrderByDescending(a => a.ChangedAt)
                        .Where(a =>
                            (a.Id == orderAttribute.AttributeId ||
                             a.MasterId.HasValue && a.MasterId.Value == orderAttribute.AttributeId) &&
                            !(a.ChangedAt > order.ChangedAt ||
                              a.DeletedAt <= order.ChangedAt))
                        .Select(Mapper.Map<Entities.Attribute, Entities.Attribute>)
                        .FirstOrDefault();

                    if (orderAttribute.Attribute != null)
                    {
                        orderAttribute.Attribute.AttributeType = types
                            .OrderByDescending(t => t.ChangedAt)
                            .FirstOrDefault(t =>
                                (t.Id == orderAttribute.Attribute.AttributeTypeId ||
                                 t.MasterId.HasValue && t.MasterId.Value == orderAttribute.Attribute.AttributeTypeId) &&
                                !(t.ChangedAt > order.ChangedAt ||
                                  t.DeletedAt <= order.ChangedAt));

                        orderAttribute.Attribute.AttributeType!.Id = orderAttribute.Attribute.AttributeType.MasterId ??
                                                                     orderAttribute.Attribute.AttributeType.Id;
                    }

                    orderAttribute.AttributeId = orderAttribute.Attribute!.MasterId ?? orderAttribute.Attribute.Id;
                }
            }

            return orders.Select(MapToDTO);
        }

        public async Task<long> CountHistoryAsync(long id)
        {
            return await DbSet.CountAsync(o => o.Id == id || o.MasterId == id);
        }

        #endregion

        #region Helpers

        private static Expression<Func<Entities.Order, Entities.Order>> ProjectOrder()
        {
            return o => new Entities.Order
            {
                Id = o.Id,
                Name = o.Name,
                Completed = o.Completed,
                Notation = o.Notation,
                ExecutionDateTime = o.ExecutionDateTime,
                OrderAttributes = o.OrderAttributes!
                    .Where(oa => oa.DeletedAt == null)
                    .Select(oa => new Entities.OrderAttribute
                    {
                        Attribute = new Entities.Attribute
                        {
                            Name = oa.Attribute!.Name,
                            AttributeType = new Entities.AttributeType
                            {
                                Name = oa.Attribute!.AttributeType!.Name,
                                Id = oa.Attribute!.AttributeType.Id,
                                DataType = oa.Attribute!.AttributeType!.DataType,
                                UsesDefinedUnits = oa.Attribute!.AttributeType!.UsesDefinedUnits,
                                UsesDefinedValues = oa.Attribute!.AttributeType!.UsesDefinedValues
                            }
                        },
                        Id = oa.Id,
                        Featured = oa.Featured,
                        AttributeId = oa.AttributeId,
                        Value = oa.Value,
                        Unit = oa.Unit,
                        UnitId = oa.UnitId,
                        ValueId = oa.ValueId,
                        CustomValue = oa.CustomValue,
                    }).ToList()
            };
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
    }
}