using System;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class OrderService : BaseService<IAppUnitOfWork>, IOrderService
    {
        public OrderService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<OrderGetDTO>> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime)
        {
            checkDatetime ??= DateTime.UtcNow;

            var items = await UnitOfWork.Orders.GetAll(pageIndex, itemsOnPage,
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

                TotalCount = await UnitOfWork.Orders.Count(hasExecutionDate, completed, 
                    searchKey, startDateTime, endDateTime)
            };
        }

        // public async Task<OrderGetDTO> GetById(long id)
        // {
        // }
        //
        // public async Task<OrderGetDTO> Create(OrderPostDTO orderPostDTO)
        // {
        // }
        //
        // public async Task Update(long id, OrderPatchDTO orderPatchDTO)
        // {
        // }
        //
        // public async Task SetCompleted(long id, OrderCompletionPatchDTO orderPatchDTO)
        // {
        // }
        //
        // public async Task Delete(long id)
        // {
        // }
        //
        // public async Task<CollectionDTO<OrderHistoryGetDTO>> GetHistory(long id)
        // {
        // }
    }
}