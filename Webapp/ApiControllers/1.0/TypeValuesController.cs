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
    public class TypeValuesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TypeValuesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeValues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeValue>>> GetTypeValues()
        {
            return await _context.TypeValues.ToListAsync();
        }

        // GET: api/TypeValues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeValue>> GetTypeValue(long id)
        {
            var typeValue = await _context.TypeValues.FindAsync(id);

            if (typeValue == null)
            {
                return NotFound();
            }

            return typeValue;
        }

        // PUT: api/TypeValues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeValue(long id, TypeValue typeValue)
        {
            if (id != typeValue.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeValue).State = EntityState.Modified;

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

        // POST: api/TypeValues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TypeValue>> PostTypeValue(TypeValue typeValue)
        {
            _context.TypeValues.Add(typeValue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeValue", new { id = typeValue.Id }, typeValue);
        }

        // DELETE: api/TypeValues/5
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
