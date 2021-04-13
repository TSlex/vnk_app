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
using PublicApi.v1.Common;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class AttributeTypeUnitsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeTypeUnitsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(long? attributeTypeId)
        {
            var query = _context.TypeUnits.AsQueryable();

            if (attributeTypeId != null)
            {
                query = _context.TypeUnits.Where(u => u.AttributeTypeId == attributeTypeId);
            }

            var units = await query.Select(u => new AttributeTypeUnitGetDTO
            {
                Id = u.Id,
                Value = u.Value
            }).ToListAsync();

            return Ok(new ResponseDTO<IEnumerable<AttributeTypeUnitGetDTO>>
            {
                Data = units
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var unit = await _context.TypeUnits.FindAsync(id);

            if (unit == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            return Ok(new ResponseDTO<AttributeTypeUnitGetDTO>
            {
                Data = new AttributeTypeUnitGetDTO
                {
                    Id = unit.Id,
                    Value = unit.Value
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create(AttributeTypeUnitPostDTO typeUnitPostDTO)
        {
            var type = await _context.AttributeTypes.FirstOrDefaultAsync(t => t.Id == typeUnitPostDTO.AttributeTypeId);

            if (type == null)
            {
                return NotFound(new ErrorResponseDTO("Тип атрибута не найден"));
            }

            var unit = new AttributeTypeUnit
            {
                Value = typeUnitPostDTO.Value,
                AttributeTypeId = typeUnitPostDTO.AttributeTypeId
            };

            await _context.TypeUnits.AddAsync(unit);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new {id = unit.Id});
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(long id, AttributeTypeUnitPatchDTO typeUnitPatchDTO)
        {
            if (id != typeUnitPatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var unit = await _context.TypeUnits.FirstOrDefaultAsync(typeUnit =>
                typeUnit.Id == typeUnitPatchDTO.Id);

            if (unit == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            unit.Value = typeUnitPatchDTO.Value;

            _context.TypeUnits.Update(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var unit = await _context.TypeUnits.FirstOrDefaultAsync(typeUnit => typeUnit.Id == id);

            if (unit == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            var type = await _context.AttributeTypes.FirstAsync(t => t.Id == unit.AttributeTypeId);

            var attributes = await _context.OrderAttributes
                .Where(attribute => attribute.UnitId == id)
                .ToListAsync();

            foreach (var attribute in attributes)
            {
                attribute.UnitId = type.DefaultUnitId;
                _context.OrderAttributes.Update(attribute);
            }

            _context.TypeUnits.Remove(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}