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
        private readonly IAppBLL _bll;

        public AttributeTypesController(IAppBLL bll)
        {
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
            var typeId = await _bll.AttributeTypes.CreateAsync(attributeTypePostDTO);
            return CreatedAtAction(nameof(GetById), await GetById(typeId, 0, 0));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, AttributeTypePatchDTO attributeTypePatchDTO)
        {
            await _bll.AttributeTypes.UpdateAsync(id, attributeTypePatchDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            await _bll.AttributeTypes.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}