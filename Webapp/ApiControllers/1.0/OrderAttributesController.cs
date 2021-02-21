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

namespace Webapp.ApiControllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderAttributesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderAttributesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderAttributes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderAttribute>>> GetOrderAttributes()
        {
            return await _context.OrderAttributes.ToListAsync();
        }

        // GET: api/OrderAttributes/5
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

        // PUT: api/OrderAttributes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/OrderAttributes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderAttribute>> PostOrderAttribute(OrderAttribute orderAttribute)
        {
            _context.OrderAttributes.Add(orderAttribute);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderAttribute", new { id = orderAttribute.Id }, orderAttribute);
        }

        // DELETE: api/OrderAttributes/5
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
