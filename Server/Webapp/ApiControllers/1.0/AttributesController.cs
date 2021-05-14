using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IAppBLL _bll;

        public AttributesController(IAppBLL bll)
        {
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
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<AttributeDetailsGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id)
        {
            return Ok(new ResponseDTO<AttributeDetailsGetDTO>
            {
                Data = await _bll.Attributes.GetByIdAsync(id)
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<AttributeTypeDetailsGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(AttributePostDTO attributePostDTO)
        {
            var attributeId = await _bll.Attributes.CreateAsync(attributePostDTO);
            return CreatedAtAction(nameof(GetById), await GetById(attributeId));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, AttributePatchDTO attributePatchDTO)
        {
            await _bll.Attributes.UpdateAsync(id, attributePatchDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            await _bll.Attributes.DeleteAsync(id);
            return NoContent();
        }
    }
}