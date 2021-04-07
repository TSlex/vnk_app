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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class TemplateAttributesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TemplateAttributesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplateAttribute>>> GetTemplateAttributes()
        {
            return await _context.TemplateAttributes.ToListAsync();
        }

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
