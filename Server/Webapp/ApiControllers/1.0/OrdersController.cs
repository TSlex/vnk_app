using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.App.Exceptions;
using BLL.Contracts;
using DAL.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using DAL.App.Entities;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAppBLL _bll;

        public OrdersController(AppDbContext context, IAppBLL bll)
        {
            _context = context;
            _bll = bll;
        }

        #region Orders

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderGetDTO>>))]
        public async Task<ActionResult> GetAllWithDate(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderGetDTO>>
            {
                Data = await _bll.Orders.GetAllAsync(pageIndex, itemsOnPage,
                    byName, true, completed, searchKey, startDateTime,
                    endDateTime, checkDatetime)
            });
        }
        
        [HttpGet("noDate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderGetDTO>>))]
        public async Task<ActionResult> GetAllWithoutDate(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, string? searchKey)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderGetDTO>>
            {
                Data = await _bll.Orders.GetAllAsync(pageIndex, itemsOnPage,
                    byName, false, completed, searchKey, null,
                    null, null)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<OrderGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id, DateTime? checkDatetime)
        {
            try
            {
                return Ok(new ResponseDTO<OrderGetDTO>
                {
                    Data = await _bll.Orders.GetByIdAsync(id, checkDatetime)
                });
            }
            
            catch (NotFoundException exception)
            {
                return NotFound(new ErrorResponseDTO(exception.Message));
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<OrderGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(OrderPostDTO orderPostDTO)
        {
            if (orderPostDTO.Attributes.Count == 0)
            {
                return BadRequest(new ErrorResponseDTO("В заказе должен быть как минимум один атрибут"));
            }

            foreach (var orderAttributePostDTO in orderPostDTO.Attributes)
            {
                var attribute =
                    await _context.Attributes
                        .Include(a => a.AttributeType)
                        .FirstOrDefaultAsync(a => a.Id == orderAttributePostDTO.AttributeId);

                if (attribute == null)
                {
                    return NotFound(new ErrorResponseDTO("Как минимум один из атрибутов неверен"));
                }

                if (attribute.AttributeType!.UsesDefinedValues)
                {
                    if (orderAttributePostDTO.ValueId == null ||
                        !await _context.TypeValues.AnyAsync(value => value.Id == orderAttributePostDTO.ValueId &&
                                                                     value.AttributeTypeId ==
                                                                     attribute.AttributeTypeId))
                    {
                        return BadRequest(new ErrorResponseDTO($"Значение атрибута '{attribute.Name}' неверно"));
                    }
                }
                else
                {
                    if (orderAttributePostDTO.CustomValue == null)
                    {
                        return BadRequest(new ErrorResponseDTO($"Значение атрибута '{attribute.Name}' неверно"));
                    }
                }

                if (attribute.AttributeType!.UsesDefinedUnits)
                {
                    if (orderAttributePostDTO.UnitId == null ||
                        !await _context.TypeUnits.AnyAsync(unit => unit.Id == orderAttributePostDTO.UnitId &&
                                                                   unit.AttributeTypeId == attribute.AttributeTypeId))
                    {
                        return BadRequest(
                            new ErrorResponseDTO($"Единица измерения атрибута '{attribute.Name}' неверна"));
                    }
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

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            await _bll.Orders.CreateAsync(orderPostDTO);

            return CreatedAtAction(nameof(GetById), await GetById(order.Id, DateTime.Now));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, OrderPatchDTO orderPatchDTO)
        {
            if (id != orderPatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var order = await _context.Orders.FirstOrDefaultAsync(t => t.Id == id);

            if (order == null)
            {
                return NotFound(new ErrorResponseDTO("Заказ не найден"));
            }

            order.Name = orderPatchDTO.Name;

            _context.Orders.Update(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(t => t.Id == id);

            if (order == null)
            {
                return NotFound(new ErrorResponseDTO("Заказ не найден"));
            }

            var orderAttributes = await _context.OrderAttributes
                .Where(ta => ta.OrderId == id)
                .ToListAsync();

            _context.OrderAttributes.RemoveRange(orderAttributes);

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region OrderAttributes

        [HttpGet("attributes")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<OrderAttributeGetDTO>>))]
        public async Task<ActionResult> GetAllOrderAttributes(long? orderId)
        {
            var query = _context.OrderAttributes.AsQueryable();

            if (orderId != null)
            {
                query = _context.OrderAttributes.Where(ta => ta.OrderId == orderId);
            }

            var attributes = await query.Select(ta => new OrderAttributeGetDTO
            {
                Id = ta.Id,
                Featured = ta.Featured,
                Name = ta.Attribute!.Name,
                Type = ta.Attribute!.AttributeType!.Name,
                TypeId = ta.Attribute!.Id,
                AttributeId = ta.AttributeId,
                DataType = (AttributeDataType) ta.Attribute!.AttributeType!.DataType,
            }).ToListAsync();

            return Ok(new ResponseDTO<IEnumerable<OrderAttributeGetDTO>>
            {
                Data = attributes
            });
        }

        [HttpGet("{orderId}/attributes")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<OrderAttributeGetDTO>>))]
        public async Task<ActionResult> GetAllOrderAttributesByOrderId(long orderId)
        {
            return await GetAllOrderAttributes(orderId);
        }

        [HttpGet("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<OrderAttributeGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetOrderAttributeById(long id)
        {
            var orderAttribute = await _context.OrderAttributes
                .Include(ta => ta.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .FirstOrDefaultAsync(ta => ta.Id == id);

            if (orderAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            return Ok(new ResponseDTO<OrderAttributeGetDTO>
            {
                Data = new OrderAttributeGetDTO
                {
                    Id = orderAttribute.Id,
                    Featured = orderAttribute.Featured,
                    Name = orderAttribute.Attribute!.Name,
                    Type = orderAttribute.Attribute!.AttributeType!.Name,
                    TypeId = orderAttribute.Attribute!.Id,
                    AttributeId = orderAttribute.AttributeId,
                    DataType = (AttributeDataType) orderAttribute.Attribute!.AttributeType!.DataType,
                }
            });
        }

        [HttpPost("{orderId}/attributes")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<OrderAttributeGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> CreateOrderAttribute(long orderId,
            OrderAttributePostDTO orderAttributePostDTO)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(t => t.Id == orderId);

            if (order == null)
            {
                return NotFound(new ErrorResponseDTO("Заказ не найден"));
            }

            var orderAttribute = new OrderAttribute
            {
                Featured = orderAttributePostDTO.Featured,
                AttributeId = orderAttributePostDTO.AttributeId,
                OrderId = orderId
            };

            await _context.OrderAttributes.AddAsync(orderAttribute);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderAttributeById),
                await GetOrderAttributeById(orderAttribute.Id));
        }

        [HttpPatch("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateOrderAttribute(long id,
            OrderAttributePatchDTO orderAttributePatchDTO)
        {
            if (id != orderAttributePatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var orderAttribute = await _context.OrderAttributes.FirstOrDefaultAsync(typeOrderAttribute =>
                typeOrderAttribute.Id == orderAttributePatchDTO.Id);

            if (orderAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            orderAttribute.Featured = orderAttributePatchDTO.Featured;
            orderAttribute.AttributeId = orderAttributePatchDTO.AttributeId;

            _context.OrderAttributes.Update(orderAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> DeleteOrderAttribute(long id)
        {
            var orderAttribute = await _context.OrderAttributes.FirstOrDefaultAsync(typeOrderAttribute =>
                typeOrderAttribute.Id == id);

            if (orderAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            _context.OrderAttributes.Remove(orderAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion
    }
}