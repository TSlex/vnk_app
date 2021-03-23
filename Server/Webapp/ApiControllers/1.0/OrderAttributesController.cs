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

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderAttributesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderAttributesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderAttribute>>> GetOrderAttributes()
        {
            return await _context.OrderAttributes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderAttribute>> GetOrderAttribute(long id)
        {
            var orderAttribute = await _context.OrderAttributes.FindAsync(id);

            if (orderAttribute == null)
            {
                return NotFound();
            }

            return orderAttribute;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderAttribute(long id, OrderAttribute orderAttribute)
        {
            if (id != orderAttribute.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderAttribute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderAttributeExists(id))
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

        [HttpPost]
        public async Task<ActionResult<OrderAttribute>> PostOrderAttribute(OrderAttribute orderAttribute)
        {
            _context.OrderAttributes.Add(orderAttribute);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderAttribute", new { id = orderAttribute.Id }, orderAttribute);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAttribute(long id)
        {
            var orderAttribute = await _context.OrderAttributes.FindAsync(id);
            if (orderAttribute == null)
            {
                return NotFound();
            }

            _context.OrderAttributes.Remove(orderAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderAttributeExists(long id)
        {
            return _context.OrderAttributes.Any(e => e.Id == id);
        }
    }
}
