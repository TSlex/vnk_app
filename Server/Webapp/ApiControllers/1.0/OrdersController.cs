using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.App.Exceptions;
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
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator, Root")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAppBLL _bll;

        public OrdersController(AppDbContext context, IAppBLL bll)
        {
            _context = context;
            _bll = bll;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderGetDTO>>))]
        public async Task<ActionResult> GetAllWithDate(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderGetDTO>>
            {
                Data = await _bll.Orders.GetAllAsync(pageIndex, itemsOnPage,
                    byName, true, completed, searchKey, startDateTime,
                    endDateTime, checkDatetime)
            });
        }

        [HttpGet("noDate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderGetDTO>>))]
        public async Task<ActionResult> GetAllWithoutDate(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, string? searchKey)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderGetDTO>>
            {
                Data = await _bll.Orders.GetAllAsync(pageIndex, itemsOnPage,
                    byName, false, completed, searchKey, null,
                    null, null)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<OrderGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id, DateTime? checkDatetime)
        {
            try
            {
                return Ok(new ResponseDTO<OrderGetDTO>
                {
                    Data = await _bll.Orders.GetByIdAsync(id, checkDatetime)
                });
            }

            catch (NotFoundException exception)
            {
                return NotFound(new ErrorResponseDTO(exception.Message));
            }
        }

        [HttpGet("{id}/history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderHistoryGetDTO>>))]
        public async Task<ActionResult> GetHistoryRecordsById(long id, int pageIndex, int itemsOnPage)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderHistoryGetDTO>>
            {
                Data = await _bll.Orders.GetHistoryAsync(id, pageIndex, itemsOnPage)
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDTO<OrderGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Create(OrderPostDTO orderPostDTO)
        {
            try
            {
                var orderId = await _bll.Orders.CreateAsync(orderPostDTO);

                return CreatedAtAction(nameof(GetById), await GetById(orderId, DateTime.Now));
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
        public async Task<IActionResult> Update(long id, OrderPatchDTO orderPatchDTO)
        {
            try
            {
                await _bll.Orders.UpdateAsync(id, orderPatchDTO);

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

        [HttpPatch("{id}/completion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> UpdateCompletion(long id, OrderCompletionPatchDTO orderCompletionPatchDTO)
        {
            try
            {
                await _bll.Orders.UpdateCompletionAsync(id, orderCompletionPatchDTO);

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
                await _bll.Orders.DeleteAsync(id);

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

        [HttpPatch("{id}/restore")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Restore(long id)
        {
            try
            {
                await _bll.Orders.RestoreAsync(id);

                return NoContent();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
        }
    }
}