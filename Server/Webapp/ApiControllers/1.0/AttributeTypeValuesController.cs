using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using PublicApi.v1;
using PublicApi.v1.Common;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class AttributeTypeValuesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeTypeValuesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(long? attributeTypeId)
        {
            var query = _context.TypeValues.AsQueryable();

            if (attributeTypeId != null)
            {
                query = _context.TypeValues.Where(u => u.AttributeTypeId == attributeTypeId);
            }

            var values = await query.Select(u => new AttributeTypeValueGetDTO
            {
                Id = u.Id,
                Value = u.Value
            }).ToListAsync();

            return Ok(new ResponseDTO<IEnumerable<AttributeTypeValueGetDTO>>
            {
                Data = values
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var value = await _context.TypeValues.FindAsync(id);

            if (value == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            return Ok(new ResponseDTO<AttributeTypeValueGetDTO>
            {
                Data = new AttributeTypeValueGetDTO
                {
                    Id = value.Id,
                    Value = value.Value
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create(AttributeTypeValuePostDTO typeValuePostDTO)
        {
            var type = await _context.AttributeTypes.FirstOrDefaultAsync(t => t.Id == typeValuePostDTO.AttributeTypeId);

            if (type == null)
            {
                return NotFound(new ErrorResponseDTO("Тип атрибута не найден"));
            }

            var value = new AttributeTypeValue
            {
                Value = typeValuePostDTO.Value,
                AttributeTypeId = typeValuePostDTO.AttributeTypeId
            };

            await _context.TypeValues.AddAsync(value);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new {id = value.Id});
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(long id, AttributeTypeValuePatchDTO typeValuePatchDTO)
        {
            if (id != typeValuePatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var value = await _context.TypeValues.FirstOrDefaultAsync(typeValue =>
                typeValue.Id == typeValuePatchDTO.Id);

            if (value == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            value.Value = typeValuePatchDTO.Value;

            _context.TypeValues.Update(value);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var value = await _context.TypeValues.FirstOrDefaultAsync(typeValue => typeValue.Id == id);

            if (value == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            var type = await _context.AttributeTypes.FirstAsync(t => t.Id == value.AttributeTypeId);
            
            if (type.DefaultValueId == id)
            {
                var newDefaultValue = await _context.TypeValues
                    .Where(v => v.AttributeTypeId == type.Id && v.Id != id)
                    .FirstOrDefaultAsync();

                if (newDefaultValue == null)
                {
                    return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
                }

                type.DefaultUnitId = newDefaultValue.Id;
                _context.AttributeTypes.Update(type);
            }

            var attributes = await _context.OrderAttributes
                .Where(attribute => attribute.ValueId == id)
                .ToListAsync();

            foreach (var attribute in attributes)
            {
                attribute.ValueId = type.DefaultValueId;
                _context.OrderAttributes.Update(attribute);
            }

            _context.TypeValues.Remove(value);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}