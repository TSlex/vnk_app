using System;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using BLL.App.Exceptions;
using BLL.Contracts.Services;
using DAL.App.DTO;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class OrderService : BaseService<IAppUnitOfWork>, IOrderService
    {
        public OrderService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<OrderGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime)
        {
            checkDatetime ??= DateTime.UtcNow;

            var items = await UnitOfWork.Orders.GetAllAsync(pageIndex, itemsOnPage,
                (DAL.App.DTO.Enums.SortOption) byName, hasExecutionDate, completed, searchKey, startDateTime,
                endDateTime);

            return new CollectionDTO<OrderGetDTO>()
            {
                Items = items.Select(o => new OrderGetDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    Completed = o.Completed,
                    Notation = o.Notation,
                    ExecutionDateTime = o.ExecutionDateTime,
                    Overdued = o.ExecutionDateTime.HasValue && o.ExecutionDateTime < checkDatetime,
                    Attributes = o.OrderAttributes!.Select(oa => new OrderAttributeGetDTO
                    {
                        Id = oa.Id,
                        Featured = oa.Featured,
                        Name = oa.Attribute!.Name,
                        Type = oa.Attribute!.AttributeType!.Name,
                        TypeId = oa.Attribute!.Id,
                        AttributeId = oa.AttributeId,
                        DataType = (AttributeDataType) oa.Attribute!.AttributeType!.DataType,
                        CustomValue = oa.CustomValue,
                        UnitId = oa.UnitId,
                        ValueId = oa.ValueId,
                        UsesDefinedUnits = oa.Attribute!.AttributeType!.UsesDefinedValues,
                        UsesDefinedValues = oa.Attribute!.AttributeType!.UsesDefinedValues
                    }).ToList()
                }),

                TotalCount = await UnitOfWork.Orders.CountAsync(hasExecutionDate, completed, 
                    searchKey, startDateTime, endDateTime)
            };
        }

        public async Task<OrderGetDTO> GetByIdAsync(long id, DateTime? checkDatetime)
        {
            if (!await UnitOfWork.Orders.ExistsAsync(id))
            {
                throw new NotFoundException("Заказ не найдет");
            }
            
            checkDatetime ??= DateTime.UtcNow;

            var item = await UnitOfWork.Orders.GetByIdAsync(id);

            return new OrderGetDTO
            {
                Id = item.Id,
                Name = item.Name,
                Completed = item.Completed,
                Notation = item.Notation,
                ExecutionDateTime = item.ExecutionDateTime,
                Overdued = item.ExecutionDateTime.HasValue && item.ExecutionDateTime < checkDatetime,
                Attributes = item.OrderAttributes!.Select(oa => new OrderAttributeGetDTO
                {
                    Id = oa.Id,
                    Featured = oa.Featured,
                    Name = oa.Attribute!.Name,
                    Type = oa.Attribute!.AttributeType!.Name,
                    TypeId = oa.Attribute!.Id,
                    AttributeId = oa.AttributeId,
                    DataType = (AttributeDataType) oa.Attribute!.AttributeType!.DataType,
                    CustomValue = oa.CustomValue,
                    UnitId = oa.UnitId,
                    ValueId = oa.ValueId,
                    UsesDefinedUnits = oa.Attribute!.AttributeType!.UsesDefinedValues,
                    UsesDefinedValues = oa.Attribute!.AttributeType!.UsesDefinedValues
                }).ToList()
            };
        }

        public Task CreateAsync(OrderPostDTO orderPostDTO)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(OrderPatchDTO orderPatchDTO)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}