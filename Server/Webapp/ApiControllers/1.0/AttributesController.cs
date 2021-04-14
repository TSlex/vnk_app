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
using Attribute = Domain.Attribute;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class AttributesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<AttributeGetDTO>>))]
        public async Task<ActionResult> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, SortOption byType, string? searchKey)
        {
            var typesQuery = _context.Attributes.Select(a => new AttributeGetDTO
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.AttributeType!.Name,
                TypeId = a.AttributeTypeId,
                DataType = (AttributeDataType) a.AttributeType!.DataType,
                UsesDefinedUnits = a.AttributeType!.UsesDefinedUnits,
                UsesDefinedValues = a.AttributeType!.UsesDefinedValues
            });

            if (!string.IsNullOrEmpty(searchKey))
            {
                typesQuery = typesQuery.Where(
                    a =>
                        a.Name.ToLower().Contains(searchKey.ToLower()) ||
                        a.Type.ToLower().Contains(searchKey.ToLower())
                );
            }
            
            typesQuery = typesQuery.OrderBy(at => at.Id);

            typesQuery = byName switch
            {
                SortOption.True => typesQuery.OrderBy(at => at.Name),
                SortOption.Reversed => typesQuery.OrderByDescending(at => at.Name),
                _ => typesQuery
            };

            typesQuery = byType switch
            {
                SortOption.True => typesQuery.OrderBy(at => at.Type),
                SortOption.Reversed => typesQuery.OrderByDescending(at => at.Type),
                _ => typesQuery
            };


            var items = new CollectionDTO<AttributeGetDTO>
            {
                TotalCount = await _context.Attributes.CountAsync(),
                Items = await typesQuery.Skip(pageIndex * itemsOnPage).Take(itemsOnPage).ToListAsync()
            };

            return Ok(new ResponseDTO<CollectionDTO<AttributeGetDTO>>
            {
                Data = items
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeGetDetailsDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id)
        {
            var item = await _context.Attributes
                .Where(a => a.Id == id)
                .Select(a => new AttributeGetDetailsDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    UsedCount = a.OrderAttributes!.Count,
                    Type = a.AttributeType!.Name,
                    TypeId = a.AttributeTypeId,
                    DataType = (AttributeDataType) a.AttributeType!.DataType,
                    DefaultUnit = a.AttributeType!.TypeUnits!
                        .Where(u => u.Id == a.AttributeType!.DefaultUnitId)
                        .Select(u => u.Value).FirstOrDefault(),
                    DefaultValue = a.AttributeType!.TypeValues!
                        .Where(v => v.Id == a.AttributeType!.DefaultValueId)
                        .Select(v => v.Value).FirstOrDefault() ?? a.AttributeType!.DefaultCustomValue,
                }).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new ErrorResponseDTO("Aтрибут не найдет"));
            }

            return Ok(new ResponseDTO<AttributeGetDetailsDTO>
            {
                Data = item
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeGetDetailsDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(AttributePostDTO dto)
        {
            var type = await _context.AttributeTypes.FirstOrDefaultAsync(at => at.Id == dto.AttributeTypeId);

            if (type == null)
            {
                return NotFound(new ErrorResponseDTO("Тип атрибута не найдет"));
            }

            var attribute = new Attribute()
            {
                Name = dto.Name,
                AttributeTypeId = dto.AttributeTypeId
            };

            await _context.Attributes.AddAsync(attribute);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), await GetById(attribute.Id));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, AttributePatchDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var type = await _context.AttributeTypes.FirstOrDefaultAsync(at => at.Id == dto.AttributeTypeId);

            if (type == null)
            {
                return NotFound(new ErrorResponseDTO("Тип атрибута не найдет"));
            }

            var attribute = await _context.Attributes
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (attribute == null)
            {
                return NotFound(new ErrorResponseDTO("Aтрибут не найдет"));
            }

            var orderAttributes = await _context.OrderAttributes
                .Where(oa => oa.AttributeId == id)
                .ToListAsync();

            attribute.Name = dto.Name;

            if (dto.AttributeTypeId != null)
            {
                if (orderAttributes.Any())
                {
                    return BadRequest(
                        new ErrorResponseDTO("Нельзя изменить тип атрибута, если атрибут уже используется"));
                }
                
                attribute.AttributeTypeId = dto.AttributeTypeId.Value;
            }

            _context.Attributes.Update(attribute);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}