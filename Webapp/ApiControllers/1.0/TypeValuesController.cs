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
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AttributeTypeValuesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeTypeValuesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttributeTypeValue>>> GetTypeValues()
        {
            return await _context.TypeValues.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AttributeTypeValue>> GetTypeValue(long id)
        {
            var typeValue = await _context.TypeValues.FindAsync(id);

            if (typeValue == null)
            {
                return NotFound();
            }

            return typeValue;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeValue(long id, AttributeTypeValue attributeTypeValue)
        {
            if (id != attributeTypeValue.Id)
            {
                return BadRequest();
            }

            _context.Entry(attributeTypeValue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeValueExists(id))
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
        public async Task<ActionResult<AttributeTypeValue>> PostTypeValue(AttributeTypeValue attributeTypeValue)
        {
            _context.TypeValues.Add(attributeTypeValue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeValue", new { id = attributeTypeValue.Id }, attributeTypeValue);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeValue(long id)
        {
            var typeValue = await _context.TypeValues.FindAsync(id);
            if (typeValue == null)
            {
                return NotFound();
            }

            _context.TypeValues.Remove(typeValue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TypeValueExists(long id)
        {
            return _context.TypeValues.Any(e => e.Id == id);
        }
    }
}
