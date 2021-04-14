using System.Collections.Generic;
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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class TemplatesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TemplatesController(AppDbContext context)
        {
            _context = context;
        }

        #region Templates

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<TemplateGetDTO>>))]
        public async Task<ActionResult> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, string? searchKey)
        {
            var typesQuery = _context.Templates.Select(t => new TemplateGetDTO
            {
                Id = t.Id,
                Name = t.Name,
                Attributes = t.TemplateAttributes!.Select(ta => new TemplateAttributeGetDTO
                {
                    Id = ta.Id,
                    Featured = ta.Featured,
                    Name = ta.Attribute!.Name,
                    Type = ta.Attribute!.AttributeType!.Name,
                    TypeId = ta.Attribute!.Id,
                    AttributeId = ta.AttributeId,
                    DataType = ta.Attribute!.AttributeType!.DataType,
                }).ToList()
            });

            if (!string.IsNullOrEmpty(searchKey))
            {
                typesQuery = typesQuery.Where(
                    a =>
                        a.Name.ToLower().Contains(searchKey.ToLower())
                );
            }

            typesQuery = typesQuery.OrderBy(at => at.Id);

            typesQuery = byName switch
            {
                SortOption.True => typesQuery.OrderBy(at => at.Name),
                SortOption.Reversed => typesQuery.OrderByDescending(at => at.Name),
                _ => typesQuery
            };

            var items = new CollectionDTO<TemplateGetDTO>
            {
                TotalCount = await _context.Templates.CountAsync(),
                Items = await typesQuery.Skip(pageIndex * itemsOnPage).Take(itemsOnPage).ToListAsync()
            };

            return Ok(new ResponseDTO<CollectionDTO<TemplateGetDTO>>
            {
                Data = items
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<TemplateGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id)
        {
            var item = await _context.Templates
                .Where(a => a.Id == id)
                .Select(t => new TemplateGetDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Attributes = t.TemplateAttributes!.Select(ta => new TemplateAttributeGetDTO
                    {
                        Id = ta.Id,
                        Featured = ta.Featured,
                        Name = ta.Attribute!.Name,
                        Type = ta.Attribute!.AttributeType!.Name,
                        TypeId = ta.Attribute!.Id,
                        AttributeId = ta.AttributeId,
                        DataType = ta.Attribute!.AttributeType!.DataType,
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new ErrorResponseDTO("Aтрибут не найдет"));
            }

            return Ok(new ResponseDTO<TemplateGetDTO>
            {
                Data = item
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<TemplateGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(TemplatePostDTO dto)
        {
            if (dto.TemplateAttributes.Count == 0)
            {
                return BadRequest(new ErrorResponseDTO("В шаблоне должен быть как минимум один атрибут"));
            }

            var attributeIds = dto.TemplateAttributes!.Select(ta => ta.AttributeId);
            var attributes = await _context.Attributes.Where(a => attributeIds.Contains(a.Id)).ToListAsync();

            if (attributes.Count != attributeIds.Count())
            {
                return NotFound(new ErrorResponseDTO("Как минимум один из атрибутов неверен"));
            }

            var template = new Template()
            {
                Name = dto.Name,
                TemplateAttributes = dto.TemplateAttributes!.Select(ta => new TemplateAttribute
                {
                    Featured = ta.Featured,
                    AttributeId = ta.AttributeId
                }).ToList()
            };

            await _context.Templates.AddAsync(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), await GetById(template.Id));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, TemplatePatchDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound(new ErrorResponseDTO("Шаблон не найден"));
            }

            template.Name = dto.Name;

            _context.Templates.Update(template);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound(new ErrorResponseDTO("Шаблон не найден"));
            }

            var templateAttributes = await _context.TemplateAttributes
                .Where(ta => ta.TemplateId == id)
                .ToListAsync();

            _context.TemplateAttributes.RemoveRange(templateAttributes);

            _context.Templates.Remove(template);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region TemplateAttributes

        [HttpGet("attributes")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<TemplateAttributeGetDTO>>))]
        public async Task<ActionResult> GetAllTemplateAttributes(long? templateId)
        {
            var query = _context.TemplateAttributes.AsQueryable();

            if (templateId != null)
            {
                query = _context.TemplateAttributes.Where(ta => ta.TemplateId == templateId);
            }

            var attributes = await query.Select(ta => new TemplateAttributeGetDTO
            {
                Id = ta.Id,
                Featured = ta.Featured,
                Name = ta.Attribute!.Name,
                Type = ta.Attribute!.AttributeType!.Name,
                TypeId = ta.Attribute!.Id,
                AttributeId = ta.AttributeId,
                DataType = ta.Attribute!.AttributeType!.DataType,
            }).ToListAsync();

            return Ok(new ResponseDTO<IEnumerable<TemplateAttributeGetDTO>>
            {
                Data = attributes
            });
        }

        [HttpGet("{templateId}/attributes")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ResponseDTO<IEnumerable<TemplateAttributeGetDTO>>))]
        public async Task<ActionResult> GetAllTemplateAttributesByTemplateId(long templateId)
        {
            return await GetAllTemplateAttributes(templateId);
        }

        [HttpGet("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<TemplateAttributeGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetTemplateAttributeById(long id)
        {
            var templateAttribute = await _context.TemplateAttributes
                .Include(ta => ta.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .FirstOrDefaultAsync(ta => ta.Id == id);

            if (templateAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            return Ok(new ResponseDTO<TemplateAttributeGetDTO>
            {
                Data = new TemplateAttributeGetDTO
                {
                    Id = templateAttribute.Id,
                    Featured = templateAttribute.Featured,
                    Name = templateAttribute.Attribute!.Name,
                    Type = templateAttribute.Attribute!.AttributeType!.Name,
                    TypeId = templateAttribute.Attribute!.Id,
                    AttributeId = templateAttribute.AttributeId,
                    DataType = templateAttribute.Attribute!.AttributeType!.DataType,
                }
            });
        }

        [HttpPost("{templateId}/attributes")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<TemplateAttributeGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> CreateTemplateAttribute(long templateId, TemplateAttributePostDTO templateAttributePostDTO)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null)
            {
                return NotFound(new ErrorResponseDTO("Шаблон не найден"));
            }

            var templateAttribute = new TemplateAttribute
            {
                Featured = templateAttributePostDTO.Featured,
                AttributeId = templateAttributePostDTO.AttributeId,
                TemplateId = templateId
            };

            await _context.TemplateAttributes.AddAsync(templateAttribute);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplateAttributeById), await GetTemplateAttributeById(templateAttribute.Id));
        }

        [HttpPatch("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateTemplateAttribute(long id,
            TemplateAttributePatchDTO templateAttributePatchDTO)
        {
            if (id != templateAttributePatchDTO.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var templateAttribute = await _context.TemplateAttributes.FirstOrDefaultAsync(typeTemplateAttribute =>
                typeTemplateAttribute.Id == templateAttributePatchDTO.Id);

            if (templateAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            templateAttribute.Featured = templateAttributePatchDTO.Featured;
            templateAttribute.AttributeId = templateAttributePatchDTO.AttributeId;

            _context.TemplateAttributes.Update(templateAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("attributes/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> DeleteTemplateAttribute(long id)
        {
            var templateAttribute = await _context.TemplateAttributes.FirstOrDefaultAsync(typeTemplateAttribute =>
                typeTemplateAttribute.Id == id);

            if (templateAttribute == null)
            {
                return NotFound(new ErrorResponseDTO("Атрибут не найден"));
            }

            _context.TemplateAttributes.Remove(templateAttribute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion
    }
}