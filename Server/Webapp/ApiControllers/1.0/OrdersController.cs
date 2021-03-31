using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PublicApi.v1;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderGetDTO>))]
        public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetOrders()
        {
            var orders = await _context.Orders.Select(o => new OrderGetDTO()
            {
                Completed = o.Completed,
                ExecutionDateTime = o.ExecutionDateTime,
                Attributes = o.OrderAttributes!.Select(oa => new OrderAttributeGetDTO()
                {
                    Name = oa.Attribute!.Name,
                    Value = oa.CustomValue ?? ""
                }).ToList()
            }).ToListAsync();

            return orders;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderGetDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderGetDTO>> GetOrder(long id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new OrderGetDTO()
                {
                    Completed = o.Completed,
                    ExecutionDateTime = o.ExecutionDateTime,
                    Attributes = o.OrderAttributes!.Select(oa => new OrderAttributeGetDTO()
                    {
                        Name = oa.Attribute!.Name,
                        Value = oa.CustomValue ?? ""
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderGetDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderGetDTO>> PostOrder(OrderPostDTO orderPostDTO)
        {
            var order = new Order()
            {
                Completed = orderPostDTO.Completed,
                ExecutionDateTime = orderPostDTO.ExecutionDateTime,
                OrderAttributes = orderPostDTO.Attributes?.Select(dto =>
                    new OrderAttribute()
                    {
                        AttributeId = dto.AttributeId,
                    }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await GetOrder(order.Id);

            return CreatedAtAction("GetOrder", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}