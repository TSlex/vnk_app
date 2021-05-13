using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using DAL.App.Entities;

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
        private readonly IAppBLL _bll;

        public AttributeTypesController(AppDbContext context, IAppBLL bll)
        {
            _context = context;
            _bll = bll;
        }

        #region AttributeTypes

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<AttributeTypeGetDTO>>))]
        public async Task<ActionResult> GetAll(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey)
        {
            return Ok(new ResponseDTO<CollectionDTO<AttributeTypeGetDTO>>
            {
                Data = await _bll.AttributeTypes.GetAllAsync(pageIndex, itemsOnPage,
                    byName, searchKey)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeTypeDetailsGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id, int valuesCount, int unitsCount)
        {
            return Ok(new ResponseDTO<AttributeTypeDetailsGetDTO>
            {
                Data = await _bll.AttributeTypes.GetByIdAsync(id, valuesCount, unitsCount)
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeDetailsGetDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(AttributeTypePostDTO attributeTypePostDTO)
        {
            // if (dto.DefaultCustomValue == null && (!dto.UsesDefinedValues || !dto.Values.Any()))
            // {
            //     return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
            // }
            //
            // if (dto.UsesDefinedValues && !dto.Values.Any())
            // {
            //     return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
            // }
            //
            // if (dto.UsesDefinedUnits && !dto.Units.Any())
            // {
            //     return BadRequest(new ErrorResponseDTO("У атрибута должна быть единица измерения по умолчанию"));
            // }
            //
            // if (dto.Units.Any() && (0 > dto.DefaultUnitIndex || dto.DefaultUnitIndex >= dto.Units.Count))
            // {
            //     return BadRequest(new ErrorResponseDTO("Индекс не соотвествует массиву единиц измерений"));
            // }
            //
            // if (dto.Values.Any() && (0 > dto.DefaultValueIndex || dto.DefaultValueIndex >= dto.Values.Count))
            // {
            //     return BadRequest(new ErrorResponseDTO("Индекс не соотвествует массиву значений"));
            // }
            //
            // var attributeType = new AttributeType()
            // {
            //     Name = dto.Name,
            //     DataType = (DAL.App.Entities.Enums.AttributeDataType) dto.DataType,
            //     DefaultCustomValue = dto.DefaultCustomValue,
            //     UsesDefinedUnits = dto.UsesDefinedUnits,
            //     UsesDefinedValues = dto.UsesDefinedValues,
            // };
            //
            // await _context.AttributeTypes.AddAsync(attributeType);
            // await _context.SaveChangesAsync();
            //
            // var values = dto.Values.Select(s =>
            //     new AttributeTypeValue {Value = s, AttributeTypeId = attributeType.Id}).ToList();
            //
            // var hasValues = dto.UsesDefinedValues && dto.Values.Any();
            // var hasUnits = dto.UsesDefinedUnits && dto.Units.Any();
            //
            // if (hasValues)
            // {
            //     foreach (var value in values)
            //     {
            //         await _context.TypeValues.AddAsync(value);
            //     }
            //
            //     await _context.SaveChangesAsync();
            // }
            //
            // var units = dto.Units.Select(s =>
            //     new AttributeTypeUnit {Value = s, AttributeTypeId = attributeType.Id}).ToList();
            //
            // if (hasUnits)
            // {
            //     foreach (var unit in units)
            //     {
            //         await _context.TypeUnits.AddAsync(unit);
            //     }
            //
            //     await _context.SaveChangesAsync();
            // }
            //
            // if (hasValues || hasUnits)
            // {
            //     attributeType =
            //         await _context.AttributeTypes.FirstAsync(t => t.Id == attributeType.Id && t.DeletedAt == null);
            // }
            //
            // if (hasValues)
            // {
            //     attributeType.DefaultValueId = values[dto.DefaultValueIndex].Id;
            // }
            //
            // if (hasUnits)
            // {
            //     attributeType.DefaultUnitId = units[dto.DefaultValueIndex].Id;
            // }
            //
            // if (hasValues || hasUnits)
            // {
            //     _context.AttributeTypes.Update(attributeType);
            //     await _context.SaveChangesAsync();
            // }
            //
            // return CreatedAtAction(nameof(GetById), await GetById(attributeType.Id, 0, 0));
            
            var typeId = await _bll.AttributeTypes.CreateAsync(attributeTypePostDTO);
            return CreatedAtAction(nameof(GetById), await GetById(typeId, 0, 0));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, AttributeTypePatchDTO attributeTypePatchDTO)
        {
            // if (id != dto.Id)
            // {
            //     return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            // }
            //
            // var attributeType = await _context.AttributeTypes.FindAsync(id);
            //
            // if (attributeType == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Тип атрибута не найдет"));
            // }
            //
            // if (attributeType.SystemicType)
            // {
            //     return NotFound(new ErrorResponseDTO("Нельзя менять системный атрибут"));
            // }
            //
            // if (dto.DefaultCustomValue == null && !attributeType.UsesDefinedValues)
            // {
            //     return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
            // }
            //
            // if (attributeType.UsesDefinedValues &&
            //     await _context.TypeValues.FirstOrDefaultAsync(v => v.Id == dto.DefaultValueId && v.DeletedAt == null) ==
            //     null)
            // {
            //     return BadRequest(new ErrorResponseDTO("Неверный идентификатор значения по умолчанию"));
            // }
            //
            // if (attributeType.UsesDefinedUnits &&
            //     await _context.TypeUnits.FirstOrDefaultAsync(u => u.Id == dto.DefaultUnitId && u.DeletedAt == null) ==
            //     null)
            // {
            //     return BadRequest(new ErrorResponseDTO("Неверный идентификатор единицы измерения по умолчанию"));
            // }
            //
            // attributeType.Name = dto.Name;
            // attributeType.DefaultCustomValue = dto.DefaultCustomValue;
            // attributeType.DefaultUnitId = dto.DefaultUnitId;
            // attributeType.DefaultValueId = dto.DefaultValueId;
            //
            // attributeType.DataType = (DAL.App.Entities.Enums.AttributeDataType) dto.DataType;
            //
            // _context.AttributeTypes.Update(attributeType);
            //
            // await _context.SaveChangesAsync();
            //
            // return NoContent();
            
            await _bll.AttributeTypes.UpdateAsync(id, attributeTypePatchDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            // var attributeType = await _context.AttributeTypes.FindAsync(id);
            //
            // if (attributeType == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            // }
            //
            // if (attributeType.SystemicType)
            // {
            //     return BadRequest(new ErrorResponseDTO("Нельзя удалить системный тип"));
            // }
            //
            // var attributes = await _context.Attributes.Where(a => a.AttributeTypeId == id && a.DeletedAt == null)
            //     .AnyAsync();
            //
            // if (attributes)
            // {
            //     return BadRequest(new ErrorResponseDTO("Нельзя удалить используемый тип"));
            // }
            //
            // _context.AttributeTypes.Remove(attributeType);
            // await _context.SaveChangesAsync();
            //
            // return NoContent();
            
            await _bll.AttributeTypes.DeleteAsync(id);
            return NoContent();
        }

        #endregion

        #region TypeValues

        [HttpGet("values")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<AttributeTypeValueGetDTO>>))]
        public async Task<ActionResult> GetAllValues(long? attributeTypeId)
        {
            var query = _context.TypeValues.AsQueryable();

            if (attributeTypeId != null)
            {
                query = _context.TypeValues.Where(v => v.AttributeTypeId == attributeTypeId && v.DeletedAt == null);
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

        [HttpGet("{attributeTypeId}/values")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<AttributeTypeValueGetDTO>>))]
        public async Task<ActionResult> GetAllValuesByTypeId(long attributeTypeId)
        {
            return await GetAllValues(attributeTypeId);
        }

        [HttpGet("values/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeTypeValueGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetValueById(long id)
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

        [HttpPost("{attributeTypeId}/values")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeValueGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> CreateValue(long attributeTypeId, AttributeTypeValuePostDTO typeValuePostDTO)
        {
            if (attributeTypeId != typeValuePostDTO.AttributeTypeId)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var type = await _context.AttributeTypes.FirstOrDefaultAsync(t =>
                t.Id == attributeTypeId && t.DeletedAt == null);

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

            return CreatedAtAction(nameof(GetValueById), await GetValueById(value.Id));
        }

        [HttpPatch("values/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateValue(long id, AttributeTypeValuePatchDTO typeValuePatchDTO)
        {
            if (id != typeValuePatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var value = await _context.TypeValues.FirstOrDefaultAsync(typeValue =>
                typeValue.Id == typeValuePatchDTO.Id && typeValue.DeletedAt == null);

            if (value == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            value.Value = typeValuePatchDTO.Value;

            _context.TypeValues.Update(value);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("values/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> DeleteValue(long id)
        {
            var value = await _context.TypeValues.FirstOrDefaultAsync(
                typeValue => typeValue.Id == id && typeValue.DeletedAt == null);

            if (value == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            var type = await _context.AttributeTypes.FirstAsync(t => t.Id == value.AttributeTypeId);

            if (type.DefaultValueId == id)
            {
                var newDefaultValue = await _context.TypeValues
                    .Where(v =>
                        v.AttributeTypeId == type.Id &&
                        v.Id != id &&
                        v.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (newDefaultValue == null)
                {
                    return BadRequest(new ErrorResponseDTO("У атрибута должно быть значение по умолчанию"));
                }

                type.DefaultUnitId = newDefaultValue.Id;
                _context.AttributeTypes.Update(type);
            }

            var attributes = await _context.OrderAttributes
                .Where(oa => oa.ValueId == id && oa.DeletedAt == null)
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

        #endregion

        #region TypeUnits

        [HttpGet("units")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<AttributeTypeUnitGetDTO>>))]
        public async Task<ActionResult> GetAllUnits(long? attributeTypeId)
        {
            var query = _context.TypeUnits.AsQueryable();

            if (attributeTypeId != null)
            {
                query = _context.TypeUnits.Where(u => u.AttributeTypeId == attributeTypeId && u.DeletedAt == null);
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

        [HttpGet("{attributeTypeId}/units")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<AttributeTypeUnitGetDTO>>))]
        public async Task<ActionResult> GetAllUnitsByTypeId(long attributeTypeId)
        {
            return await GetAllUnits(attributeTypeId);
        }

        [HttpGet("units/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeTypeUnitGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetUnitById(long id)
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

        [HttpPost("{attributeTypeId}/units")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeUnitGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> CreateUnit(long attributeTypeId, AttributeTypeUnitPostDTO typeUnitPostDTO)
        {
            if (attributeTypeId != typeUnitPostDTO.AttributeTypeId)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var type = await _context.AttributeTypes.FirstOrDefaultAsync(t =>
                t.Id == attributeTypeId && t.DeletedAt == null);

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

            return CreatedAtAction(nameof(GetUnitById), await GetUnitById(unit.Id));
        }

        [HttpPatch("units/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateUnit(long id, AttributeTypeUnitPatchDTO typeUnitPatchDTO)
        {
            if (id != typeUnitPatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var unit = await _context.TypeUnits.FirstOrDefaultAsync(u =>
                u.Id == typeUnitPatchDTO.Id && u.DeletedAt == null);

            if (unit == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            unit.Value = typeUnitPatchDTO.Value;

            _context.TypeUnits.Update(unit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("units/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> DeleteUnit(long id)
        {
            var unit = await _context.TypeUnits.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

            if (unit == null)
            {
                return NotFound(new ErrorResponseDTO("Единица измерения не найдена"));
            }

            var type = await _context.AttributeTypes.FirstAsync(
                t => t.Id == unit.AttributeTypeId && t.DeletedAt == null);

            if (type.DefaultUnitId == id)
            {
                var newDefaultUnit = await _context.TypeUnits
                    .Where(u => u.AttributeTypeId == type.Id && u.Id != id && u.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (newDefaultUnit == null)
                {
                    return BadRequest(new ErrorResponseDTO("У атрибута должна быть единица измерения по умолчанию"));
                }

                type.DefaultUnitId = newDefaultUnit.Id;
                _context.AttributeTypes.Update(type);
            }

            var attributes = await _context.OrderAttributes
                .Where(oa => oa.UnitId == id && oa.DeletedAt == null)
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

        #endregion
    }
}