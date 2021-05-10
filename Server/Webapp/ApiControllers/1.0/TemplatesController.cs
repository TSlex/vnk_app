using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.App.Exceptions;
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
    public class TemplatesController : ControllerBase
    {
        private readonly IAppBLL _bll;

        public TemplatesController(IAppBLL bll)
        {
            _bll = bll;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<TemplateGetDTO>>))]
        public async Task<ActionResult> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, string? searchKey)
        {
            return Ok(new ResponseDTO<CollectionDTO<TemplateGetDTO>>
            {
                Data = await _bll.Templates.GetAllAsync(pageIndex, itemsOnPage,
                    byName, searchKey)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<TemplateGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id)
        {
            try
            {
                return Ok(new ResponseDTO<TemplateGetDTO>
                {
                    Data = await _bll.Templates.GetByIdAsync(id)
                });
            }

            catch (NotFoundException exception)
            {
                return NotFound(new ErrorResponseDTO(exception.Message));
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<TemplateGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(TemplatePostDTO templatePostDTO)
        {
            try
            {
                var orderId = await _bll.Templates.CreateAsync(templatePostDTO);

                return CreatedAtAction(nameof(GetById), await GetById(orderId));
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException notFoundException)
            {
                return BadRequest(new ErrorResponseDTO(notFoundException.Message));
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, TemplatePatchDTO templatePatchDTO)
        {
            try
            {
                await _bll.Templates.UpdateAsync(id, templatePatchDTO);

                return NoContent();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException notFoundException)
            {
                return BadRequest(new ErrorResponseDTO(notFoundException.Message));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _bll.Templates.DeleteAsync(id);

                return NoContent();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException notFoundException)
            {
                return BadRequest(new ErrorResponseDTO(notFoundException.Message));
            }
        }
    }
}