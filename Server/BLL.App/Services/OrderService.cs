﻿using System;
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
                Items = items.Select(item => MapToGetDTO(item, checkDatetime)),

                TotalCount = await UnitOfWork.Orders.CountAsync(hasExecutionDate, completed,
                    searchKey, startDateTime, endDateTime)
            };
        }

        public async Task<OrderGetDTO> GetByIdAsync(long id, DateTime? checkDatetime)
        {
            if (!await UnitOfWork.Orders.AnyAsync(id))
            {
                throw new NotFoundException("Заказ не найдет");
            }

            checkDatetime ??= DateTime.UtcNow;

            var item = await UnitOfWork.Orders.GetByIdAsync(id);

            return MapToGetDTO(item, checkDatetime);
        }

        public async Task<long> CreateAsync(OrderPostDTO orderPostDTO)
        {
            if (orderPostDTO.Attributes.Count == 0)
            {
                throw new ValidationException("В заказе должен быть как минимум один атрибут");
            }

            foreach (var orderAttributePostDTO in orderPostDTO.Attributes)
            {
                var attribute = await UnitOfWork.Attributes.GetByIdWithType(orderAttributePostDTO.AttributeId);

                if (attribute == null)
                {
                    throw new ValidationException("Как минимум один из атрибутов неверен");
                }

                if (attribute.AttributeType!.UsesDefinedValues)
                {
                    if (!orderAttributePostDTO.ValueId.HasValue ||
                        !await UnitOfWork.AttributeTypeValues.AnyAsync(orderAttributePostDTO.ValueId!.Value,
                            attribute.AttributeTypeId))
                    {
                        throw new ValidationException($"Значение атрибута '{attribute.Name}' неверно");
                    }
                }
                else
                {
                    orderAttributePostDTO.ValueId = null;

                    if (orderAttributePostDTO.CustomValue == null)
                    {
                        throw new ValidationException($"Значение атрибута '{attribute.Name}' неверно");
                    }
                }

                if (attribute.AttributeType!.UsesDefinedUnits)
                {
                    if (!orderAttributePostDTO.UnitId.HasValue ||
                        !await UnitOfWork.AttributeTypeUnits.AnyAsync(orderAttributePostDTO.UnitId!.Value,
                            attribute.AttributeTypeId))
                    {
                        throw new ValidationException($"Единица измерения атрибута '{attribute.Name}' неверна");
                    }
                }
                else
                {
                    orderAttributePostDTO.UnitId = null;
                }
            }

            var order = new Order()
            {
                Name = orderPostDTO.Name,
                Completed = orderPostDTO.Completed,
                Notation = orderPostDTO.Notation,
                ExecutionDateTime = orderPostDTO.ExecutionDateTime,
                OrderAttributes = orderPostDTO.Attributes!.Select(oa => new OrderAttribute
                {
                    Featured = oa.Featured,
                    AttributeId = oa.AttributeId,
                    ValueId = oa.ValueId,
                    UnitId = oa.UnitId,
                    CustomValue = oa.CustomValue,
                }).ToList()
            };

            var orderIdCallBack = await UnitOfWork.Orders.AddAsync(order);

            await UnitOfWork.SaveChangesAsync();

            return orderIdCallBack();
        }

        public async Task UpdateAsync(long id, OrderPatchDTO orderPatchDTO)
        {
            var order = await ValidateAndReturnOrderAsync(id, orderPatchDTO.Id);

            order.Name = orderPatchDTO.Name;
            order.Completed = orderPatchDTO.Completed;
            order.Notation = orderPatchDTO.Notation;
            order.ExecutionDateTime = orderPatchDTO.ExecutionDateTime;

            await UnitOfWork.Orders.UpdateAsync(order);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCompletionAsync(long id, OrderCompletionPatchDTO orderCompletionPatchDTO)
        {
            var order = await ValidateAndReturnOrderAsync(id, orderCompletionPatchDTO.Id);

            order.Completed = orderCompletionPatchDTO.Completed;

            await UnitOfWork.Orders.UpdateAsync(order);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var order = await ValidateAndReturnOrderAsync(id);

            var orderAttributes = await UnitOfWork.OrderAttributes.GetAllByOrderId(id);

            await UnitOfWork.OrderAttributes.RemoveRangeAsync(orderAttributes);
            await UnitOfWork.Orders.RemoveAsync(order);

            await UnitOfWork.SaveChangesAsync();
        }

        #region Helpers

        private static OrderGetDTO MapToGetDTO(Order item, DateTime? checkDatetime)
        {
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

        private Task<Order> ValidateAndReturnOrderAsync(long id)
        {
            return ValidateAndReturnOrderAsync(id, id);
        }

        private async Task<Order> ValidateAndReturnOrderAsync(long id, long orderDTOId)
        {
            if (id != orderDTOId)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var order = await UnitOfWork.Orders.FirstOrDefaultAsync(id);

            if (order == null)
            {
                throw new NotFoundException("Заказ не найден");
            }

            return order;
        }

        #endregion
    }
}