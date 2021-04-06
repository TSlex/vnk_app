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
using PublicApi.v1.Enums;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class AttributeTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CollectionDTO<AttributeTypeGetDTO>))]
        public async Task<ActionResult<CollectionDTO<AttributeTypeGetDTO>>> GetAll(long page, int itemsCount)
        {
            var items = new CollectionDTO<AttributeTypeGetDTO>
            {
                PageIndex = page,
                TotalCount = await _context.AttributeTypes.CountAsync(),
                // PagesCount = (long) Math.Ceiling(await _context.AttributeTypes.CountAsync() / (1.0f * itemsCount)),
                Items = await _context.AttributeTypes.Select(at => new AttributeTypeGetDTO
                    {
                        Id = at.Id,
                        Name = at.Name,
                        DataType = (AttributeDataType) at.DataType,
                        SystemicType = at.SystemicType,
                        UsedCount = at.Attributes!.Count,
                        UsesDefinedUnits = at.UsesDefinedUnits,
                        UsesDefinedValues = at.UsesDefinedValues
                    }).OrderBy(at => at.Name)
                    .Take(itemsCount)
                    .ToListAsync()
            };

            return items;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AttributeTypeGetDetailsDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AttributeTypeGetDetailsDTO>> GetById(long id, int valuesCount, int unitsCount)
        {
            var item = await _context.AttributeTypes
                .Where(at => at.Id == id)
                .Select(at => new AttributeTypeGetDetailsDTO
                {
                    Id = at.Id,
                    Name = at.Name,
                    Units = at.TypeUnits!.Select(u => new AttributeTypeUnitDetailsDTO
                        {
                            Id = u.Id,
                            Value = u.Value
                        }).OrderBy(u => u.Value)
                        .Take(unitsCount)
                        .ToList(),
                    Values = at.TypeValues!.Select(v => new AttributeTypeValueDetailsDTO
                        {
                            Id = v.Id,
                            Value = v.Value
                        }).OrderBy(v => v.Value)
                        .Take(valuesCount)
                        .ToList(),
                    DataType = (AttributeDataType) at.DataType,
                    SystemicType = at.SystemicType,
                    UnitsCount = at.TypeUnits!.Count,
                    UsedCount = at.Attributes!.Count,
                    ValuesCount = at.TypeValues!.Count,
                    DefaultCustomValue = at.DefaultCustomValue ?? "???",
                    DefaultUnitId = at.DefaultUnitId,
                    DefaultValueId = at.DefaultValueId,
                    UsesDefinedUnits = at.UsesDefinedUnits,
                    UsesDefinedValues = at.UsesDefinedValues
                }).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderGetDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AttributeType>> PostAttributeType(AttributeTypePostDTO dto)
        {
            var attributeType = new AttributeType()
            {
                Name = dto.Name,
                DataType = (Domain.Enums.AttributeDataType) dto.DataType,
                TypeUnits = dto.Units?.Select(s => new AttributeTypeUnit {Value = s}).ToList(),
                TypeValues = dto.Values?.Select(s => new AttributeTypeValue {Value = s}).ToList(),
                DefaultCustomValue = dto.DefaultCustomValue,
                DefaultUnitId = dto.DefaultUnitId,
                DefaultValueId = dto.DefaultValueId,
                UsesDefinedUnits = dto.UsesDefinedUnits,
                UsesDefinedValues = dto.UsesDefinedValues,
            };

            await _context.AttributeTypes.AddAsync(attributeType);
            await _context.SaveChangesAsync();

            var item = await GetById(attributeType.Id, 0, 0);

            return CreatedAtAction("GetById", item);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAttributeType(long id, AttributeTypePutDTO dto)
        {
            var attributeType = await _context.AttributeTypes.FindAsync(id);

            if (attributeType == null)
            {
                return NotFound();
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            attributeType.Name = dto.Name;
            attributeType.DefaultCustomValue = dto.DefaultCustomValue;
            attributeType.DefaultUnitId = dto.DefaultUnitId;
            attributeType.DefaultValueId = dto.DefaultValueId;
            attributeType.UsesDefinedUnits = dto.UsesDefinedUnits;
            attributeType.UsesDefinedValues = dto.UsesDefinedValues;
            attributeType.DataType = (Domain.Enums.AttributeDataType) dto.DataType;

            _context.AttributeTypes.Update(attributeType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributeType(long id)
        {
            var attributeType = await _context.AttributeTypes.FindAsync(id);
            if (attributeType == null)
            {
                return NotFound();
            }

            _context.AttributeTypes.Remove(attributeType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}