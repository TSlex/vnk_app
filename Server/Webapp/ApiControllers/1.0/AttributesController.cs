using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Attribute = DAL.App.Entities.Attribute;

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
        private readonly AppBLL _bll;

        public AttributesController(AppDbContext context, AppBLL bll)
        {
            _context = context;
            _bll = bll;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<AttributeGetDTO>>))]
        public async Task<ActionResult> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, SortOption byType, string? searchKey)
        {
            return Ok(new ResponseDTO<CollectionDTO<AttributeGetDTO>>
            {
                Data = await _bll.Attributes.GetAllAsync(pageIndex, itemsOnPage,
                    byName, byType, searchKey)
            });
            
            // var typesQuery = _context.Attributes
            //     .Where(a => a.DeletedAt == null)
            //     .Select(a => new AttributeGetDTO
            //     {
            //         Id = a.Id,
            //         Name = a.Name,
            //         Type = a.AttributeType!.Name,
            //         TypeId = a.AttributeTypeId,
            //         DataType = (AttributeDataType) a.AttributeType!.DataType,
            //         UsesDefinedUnits = a.AttributeType!.UsesDefinedUnits,
            //         UsesDefinedValues = a.AttributeType!.UsesDefinedValues
            //     });
            //
            // if (!string.IsNullOrEmpty(searchKey))
            // {
            //     typesQuery = typesQuery.Where(
            //         a =>
            //             a.Name.ToLower().Contains(searchKey.ToLower()) ||
            //             a.Type.ToLower().Contains(searchKey.ToLower())
            //     );
            // }
            //
            // typesQuery = typesQuery.OrderBy(at => at.Id);
            //
            // typesQuery = byName switch
            // {
            //     SortOption.True => typesQuery.OrderBy(at => at.Name),
            //     SortOption.Reversed => typesQuery.OrderByDescending(at => at.Name),
            //     _ => typesQuery
            // };
            //
            // typesQuery = byType switch
            // {
            //     SortOption.True => typesQuery.OrderBy(at => at.Type),
            //     SortOption.Reversed => typesQuery.OrderByDescending(at => at.Type),
            //     _ => typesQuery
            // };
            //
            //
            // var items = new CollectionDTO<AttributeGetDTO>
            // {
            //     TotalCount = await _context.Attributes.CountAsync(a => a.DeletedAt == null),
            //     Items = await typesQuery.Skip(pageIndex * itemsOnPage).Take(itemsOnPage).ToListAsync()
            // };
            //
            // return Ok(new ResponseDTO<CollectionDTO<AttributeGetDTO>>
            // {
            //     Data = items
            // });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeGetDetailsDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id)
        {
            return Ok(new ResponseDTO<AttributeGetDetailsDTO>
            {
                Data = await _bll.Attributes.GetByIdAsync(id)
            });
            
            // var item = await _context.Attributes
            //     .Where(a => a.Id == id)
            //     .Select(a => new AttributeGetDetailsDTO
            //     {
            //         Id = a.Id,
            //         Name = a.Name,
            //         UsedCount = a.OrderAttributes!.Count(oa => oa.DeletedAt == null),
            //         Type = a.AttributeType!.Name,
            //         TypeId = a.AttributeTypeId,
            //         DataType = (AttributeDataType) a.AttributeType!.DataType,
            //         DefaultUnit = a.AttributeType!.TypeUnits!
            //             .Where(u => u.Id == a.AttributeType!.DefaultUnitId)
            //             .Select(u => u.Value).FirstOrDefault(),
            //         DefaultValue = a.AttributeType!.TypeValues!
            //             .Where(v => v.Id == a.AttributeType!.DefaultValueId)
            //             .Select(v => v.Value).FirstOrDefault() ?? a.AttributeType!.DefaultCustomValue,
            //     }).FirstOrDefaultAsync();
            //
            // if (item == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Aтрибут не найдет"));
            // }
            //
            // return Ok(new ResponseDTO<AttributeGetDetailsDTO>
            // {
            //     Data = item
            // });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeDetailsGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(AttributePostDTO attributePostDTO)
        {
            var attributeId = await _bll.Attributes.CreateAsync(attributePostDTO);
            return CreatedAtAction(nameof(GetById), await GetById(attributeId));
            
            // var type = await _context.AttributeTypes.FirstOrDefaultAsync(at =>
            //     at.Id == dto.AttributeTypeId && at.DeletedAt == null);
            //
            // if (type == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Тип атрибута не найдет"));
            // }
            //
            // var attribute = new Attribute()
            // {
            //     Name = dto.Name,
            //     AttributeTypeId = dto.AttributeTypeId
            // };
            //
            // await _context.Attributes.AddAsync(attribute);
            // await _context.SaveChangesAsync();
            //
            // return CreatedAtAction(nameof(GetById), await GetById(attribute.Id));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, AttributePatchDTO attributePatchDTO)
        {
            await _bll.Attributes.UpdateAsync(id, attributePatchDTO);
            return NoContent();
            
            // if (id != dto.Id)
            // {
            //     return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            // }
            //
            // var type = await _context.AttributeTypes.FirstOrDefaultAsync(at =>
            //     at.Id == dto.AttributeTypeId && at.DeletedAt == null);
            //
            // if (type == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Тип атрибута не найдет"));
            // }
            //
            // var attribute = await _context.Attributes
            //     .Where(a => a.Id == id && a.DeletedAt == null)
            //     .FirstOrDefaultAsync();
            //
            // if (attribute == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Aтрибут не найдет"));
            // }
            //
            // var orderAttributes = await _context.OrderAttributes
            //     .Where(oa => oa.AttributeId == id && oa.DeletedAt == null)
            //     .ToListAsync();
            //
            // attribute.Name = dto.Name;
            //
            // if (dto.AttributeTypeId != null)
            // {
            //     if (orderAttributes.Any(oa => oa.DeletedAt == null))
            //     {
            //         return BadRequest(
            //             new ErrorResponseDTO("Нельзя изменить тип используемого атрибута"));
            //     }
            //
            //     attribute.AttributeTypeId = dto.AttributeTypeId.Value;
            // }
            //
            // _context.Attributes.Update(attribute);
            //
            // await _context.SaveChangesAsync();
            //
            // return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            await _bll.Attributes.DeleteAsync(id);
            return NoContent();
            // var attribute = await _context.Attributes.FindAsync(id);
            //
            // if (attribute == null)
            // {
            //     return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            // }
            //
            // var attributes = await _context.OrderAttributes.Where(oa => oa.AttributeId == id && oa.DeletedAt == null)
            //     .AnyAsync();
            //
            // if (attributes)
            // {
            //     return BadRequest(new ErrorResponseDTO("Нельзя удалить используемый тип"));
            // }
            //
            // _context.Attributes.Remove(attribute);
            // await _context.SaveChangesAsync();
            //
            // return NoContent();
        }
    }
}