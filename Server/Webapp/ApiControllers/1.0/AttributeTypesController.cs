using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
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
    // [Authorize(Roles = "User, Administrator, Root")]
    public class AttributeTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributeTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<AttributeTypeGetDTO>>))]
        public async Task<ActionResult<CollectionDTO<AttributeTypeGetDTO>>> GetAll(int pageIndex, int itemsCount,
            bool reversed)
        {
            var typesQuery = _context.AttributeTypes.Select(at => new AttributeTypeGetDTO
            {
                Id = at.Id,
                Name = at.Name,
                DataType = (AttributeDataType) at.DataType,
                SystemicType = at.SystemicType,
                UsedCount = at.Attributes!.Count,
                UsesDefinedUnits = at.UsesDefinedUnits,
                UsesDefinedValues = at.UsesDefinedValues
            });

            typesQuery = typesQuery.OrderBy(at => at.Id);
            typesQuery = reversed ? typesQuery.OrderByDescending(at => at.Name) : typesQuery.OrderBy(at => at.Name);

            var items = new CollectionDTO<AttributeTypeGetDTO>
            {
                TotalCount = await _context.AttributeTypes.CountAsync(),
                Items = await typesQuery.Skip(pageIndex * itemsCount).Take(itemsCount).ToListAsync()
            };

            return Ok(new ResponseDTO<CollectionDTO<AttributeTypeGetDTO>>
            {
                Data = items
            });
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeGetDetailsDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult<AttributeType>> PostAttributeType(AttributeTypePostDTO dto)
        {
            if (dto.DefaultCustomValue == null && !dto.UsesDefinedValues || !dto.Values.Any())
            {
                return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
            }

            if (dto.UsesDefinedValues && !dto.Values.Any())
            {
                return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
            }

            if (dto.UsesDefinedUnits && !dto.Units.Any())
            {
                return BadRequest(new ErrorResponseDTO("У атрибута должна быть единица измерения по умолчанию"));
            }

            if (dto.Units.Any() && (0 < dto.DefaultUnitIndex || dto.DefaultUnitIndex >= dto.Units.Count))
            {
                return BadRequest(new ErrorResponseDTO("Индекс не соотвествует массиву единиц измерений"));
            }

            if (dto.Values.Any() && (0 < dto.DefaultValueIndex || dto.DefaultValueIndex >= dto.Values.Count))
            {
                return BadRequest(new ErrorResponseDTO("Индекс не соотвествует массиву значений"));
            }

            var attributeType = new AttributeType()
            {
                Name = dto.Name,
                DataType = (Domain.Enums.AttributeDataType) dto.DataType,
                DefaultCustomValue = dto.DefaultCustomValue,
                UsesDefinedUnits = dto.UsesDefinedUnits,
                UsesDefinedValues = dto.UsesDefinedValues,
            };

            await _context.AttributeTypes.AddAsync(attributeType);
            await _context.SaveChangesAsync();

            var values = dto.Values.Select(s =>
                new AttributeTypeValue {Value = s, AttributeTypeId = attributeType.Id}).ToList();

            var hasValues = dto.UsesDefinedValues && dto.Values.Any();
            var hasUnits = dto.UsesDefinedUnits && dto.Units.Any();

            if (hasValues)
            {
                foreach (var value in values)
                {
                    await _context.TypeValues.AddAsync(value);
                }

                await _context.SaveChangesAsync();
            }

            var units = dto.Units.Select(s =>
                new AttributeTypeUnit {Value = s, AttributeTypeId = attributeType.Id}).ToList();

            if (hasUnits)
            {
                foreach (var unit in units)
                {
                    await _context.TypeUnits.AddAsync(unit);
                }

                await _context.SaveChangesAsync();
            }

            if (hasValues || hasUnits)
            {
                attributeType = await _context.AttributeTypes.FirstAsync(type => type.Id == attributeType.Id);
            }

            if (hasValues)
            {
                attributeType.DefaultValueId = values[dto.DefaultValueIndex].Id;
            }

            if (hasUnits)
            {
                attributeType.DefaultUnitId = units[dto.DefaultValueIndex].Id;
            }

            if (hasValues || hasUnits)
            {
                _context.AttributeTypes.Update(attributeType);
                await _context.SaveChangesAsync();
            }

            var item = await GetById(attributeType.Id, 0, 0);

            return CreatedAtAction("GetById", item);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchAttributeType(long id, AttributeTypePutDTO dto)
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

        //TODO: Make delete possible
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