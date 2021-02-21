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
    public class TemplateAttributesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TemplateAttributesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TemplateAttributes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplateAttribute>>> GetTemplateAttributes()
        {
            return await _context.TemplateAttributes.ToListAsync();
        }

        // GET: api/TemplateAttributes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateAttribute>> GetTemplateAttribute(long id)
        {
            var templateAttribute = await _context.TemplateAttributes.FindAsync(id);

            if (templateAttribute == null)
            {
                return NotFound();
            }

            return templateAttribute;
        }

        // PUT: api/TemplateAttributes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplateAttribute(long id, TemplateAttribute templateAttribute)
        {
            if (id != templateAttribute.Id)
            {
                return BadRequest();
            }

            _context.Entry(templateAttribute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplateAttributeExists(id))
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

        // POST: api/TemplateAttributes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TemplateAttribute>> PostTemplateAttribute(TemplateAttribute templateAttribute)
        {
            _context.TemplateAttributes.Add(templateAttribute);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTemplateAttribute", new { id = templateAttribute.Id }, templateAttribute);
        }

        // DELETE: api/TemplateAttributes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplateAttribute(long id)
        {
            var templateAttribute = await _context.TemplateAttributes.FindAsync(id);
            if (templateAttribute == null)
            {
                return NotFound();
            }

            _context.TemplateAttributes.Remove(templateAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TemplateAttributeExists(long id)
        {
            return _context.TemplateAttributes.Any(e => e.Id == id);
        }
    }
}
