﻿using System;
using System.Collections.Generic;
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
                Items = items.Select(item => MapOrderToGetDTO(item, checkDatetime)),

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

            return MapOrderToGetDTO(item, checkDatetime);
        }

        public async Task<CollectionDTO<OrderHistoryGetDTO>> GetHistoryAsync(long id, int pageIndex, int itemsOnPage)
        {
            if (!await UnitOfWork.Orders.AnyIncludeDeletedAsync(id))
            {
                throw new NotFoundException("Заказ не найдет");
            }

            return new CollectionDTO<OrderHistoryGetDTO>()
            {
                Items = (await UnitOfWork.Orders.GetHistoryAsync(id, pageIndex, itemsOnPage)).Select(item =>
                    new OrderHistoryGetDTO
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Completed = item.Completed,
                        Notation = item.Notation,
                        ExecutionDateTime = item.ExecutionDateTime,
                        DeletedAt = item.DeletedAt,
                        DeletedBy = item.DeletedBy,
                        ChangedAt = item.ChangedAt,
                        ChangedBy = item.ChangedBy,
                        CreatedAt = item.CreatedAt,
                        CreatedBy = item.CreatedBy,
                        Attributes = item.OrderAttributes!.Select(MapAttributesToDTO).ToList()
                    }),
                TotalCount = await UnitOfWork.Orders.CountHistoryAsync(id)
            };
        }

        public async Task<long> CreateAsync(OrderPostDTO orderPostDTO)
        {
            if (orderPostDTO.Attributes.Count == 0)
            {
                throw new ValidationException("В заказе должен быть как минимум один атрибут");
            }

            foreach (var orderAttributePostDTO in orderPostDTO.Attributes)
            {
                await ValidateAndFormatAttribute(orderAttributePostDTO);
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
            
            int orderAttributesCount = await UnitOfWork.OrderAttributes.CountByOrderId(id);

            foreach (var attributePatchDTO in orderPatchDTO.Attributes)
            {
                OrderAttribute attribute;
                
                switch (attributePatchDTO.PatchOption)
                {
                    case PatchOption.Updated:
                        if (!(attributePatchDTO.Id.HasValue &&
                              await UnitOfWork.OrderAttributes.AnyAsync(attributePatchDTO.Id.Value, id)))
                        {
                            throw new NotFoundException("Атрибут не найден");
                        }
                        
                        await ValidateAndFormatAttribute(attributePatchDTO);

                        attribute = await UnitOfWork.OrderAttributes.FirstOrDefaultAsync(attributePatchDTO.Id.Value)!;

                        attribute.Featured = attributePatchDTO.Featured;
                        attribute.AttributeId = attributePatchDTO.AttributeId;
                        attribute.CustomValue = attributePatchDTO.CustomValue;
                        attribute.ValueId = attributePatchDTO.ValueId;
                        attribute.UnitId = attributePatchDTO.UnitId;

                        await UnitOfWork.OrderAttributes.UpdateAsync(attribute);
                        break;
                    
                    case PatchOption.Created:
                        
                        await ValidateAndFormatAttribute(attributePatchDTO);
                        
                        attribute = new OrderAttribute()
                        {
                            Featured = attributePatchDTO.Featured,
                            AttributeId = attributePatchDTO.AttributeId,
                            CustomValue = attributePatchDTO.CustomValue,
                            ValueId = attributePatchDTO.ValueId,
                            UnitId = attributePatchDTO.UnitId,
                            OrderId = order.Id
                        };

                        await UnitOfWork.OrderAttributes.AddAsync(attribute);
                        break;
                    
                    case PatchOption.Deleted:
                        if (!(attributePatchDTO.Id.HasValue &&
                              await UnitOfWork.OrderAttributes.AnyAsync(attributePatchDTO.Id.Value)))
                        {
                            throw new NotFoundException("Атрибут не найден");
                        }
                        
                        if (orderAttributesCount <= 1)
                        {
                            throw new ValidationException("В заказе должен быть как минимум один атрибут");
                        }

                        await UnitOfWork.OrderAttributes.RemoveAsync(attributePatchDTO.Id.Value);

                        orderAttributesCount--;
                        
                        break;
                }
            }

            await UnitOfWork.SaveChangesAsync();
        }

        private async Task ValidateAndFormatAttribute(OrderAttributePatchDTO orderAttributePostDTO)
        {
            await ValidateAndFormatAttribute(new OrderAttributePostDTO
            {
                Featured = orderAttributePostDTO.Featured,
                AttributeId = orderAttributePostDTO.AttributeId,
                CustomValue = orderAttributePostDTO.CustomValue,
                UnitId = orderAttributePostDTO.UnitId,
                ValueId = orderAttributePostDTO.ValueId
            });
        }

        private async Task ValidateAndFormatAttribute(OrderAttributePostDTO orderAttributePostDTO)
        {
            var attribute = await UnitOfWork.Attributes.GetByIdWithType(orderAttributePostDTO.AttributeId);

            if (attribute == null)
            {
                throw new ValidationException("Один из создаваемых атрибутов неверен");
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

        public async Task RestoreAsync(long id)
        {
            var orderExists = await UnitOfWork.Orders.AnyIncludeDeletedAsync(id);

            if (!orderExists)
            {
                throw new NotFoundException("Заказ не найден");
            }

            await UnitOfWork.Orders.RestoreAsync(id);

            await UnitOfWork.SaveChangesAsync();
        }

        #region Helpers

        private static OrderGetDTO MapOrderToGetDTO(Order item, DateTime? checkDatetime)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Completed = item.Completed,
                Notation = item.Notation,
                ExecutionDateTime = item.ExecutionDateTime,
                Overdued = item.ExecutionDateTime.HasValue && item.ExecutionDateTime < checkDatetime,
                Attributes = item.OrderAttributes!.Select(MapAttributesToDTO).ToList()
            };
        }

        private static OrderAttributeGetDTO MapAttributesToDTO(OrderAttribute oa)
        {
            return new()
            {
                Id = oa.Id,
                Featured = oa.Featured,
                Name = oa.Attribute!.Name,
                Type = oa.Attribute!.AttributeType!.Name,
                TypeId = oa.Attribute!.Id,
                AttributeId = oa.AttributeId,
                DataType = (AttributeDataType) oa.Attribute!.AttributeType!.DataType,
                Value = oa.Value?.Value ?? oa.CustomValue ?? "",
                Unit = oa.Unit?.Value ?? "",
                UnitId = oa.UnitId,
                ValueId = oa.ValueId,
                UsesDefinedUnits = oa.Attribute!.AttributeType!.UsesDefinedValues,
                UsesDefinedValues = oa.Attribute!.AttributeType!.UsesDefinedValues
            };
        }

        private async Task<Order> ValidateAndReturnOrderAsync(long id)
        {
            var order = await UnitOfWork.Orders.FirstOrDefaultAsync(id);

            if (order == null)
            {
                throw new NotFoundException("Заказ не найден");
            }

            return order;
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