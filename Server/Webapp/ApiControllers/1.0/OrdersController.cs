using System;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using AppAPI._1._0.Responses;
using BLL.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapp.ApiControllers._1._0
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize(Roles = "User, Administrator, Root")]
    public class OrdersController : ControllerBase
    {
        private readonly IAppBLL _bll;

        public OrdersController(IAppBLL bll)
        {
            _bll = bll;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<CollectionDTO<OrderGetDTO>>))]
        public async Task<ActionResult> GetAllWithDate(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, bool? overdued, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime)
        {
            return Ok(new ResponseDTO<CollectionDTO<OrderGetDTO>>
            {
                Data = await _bll.Orders.GetAllAsync(pageIndex, itemsOnPage,
                    byName, true, completed, overdued, searchKey, startDateTime,
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
                    byName, false, completed, null, searchKey, null,
                    null, null)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<OrderGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetById(long id, DateTime? checkDatetime)
        {
            return Ok(new ResponseDTO<OrderGetDTO>
            {
                Data = await _bll.Orders.GetByIdAsync(id, checkDatetime)
            });
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
            var orderId = await _bll.Orders.CreateAsync(orderPostDTO);
            return CreatedAtAction(nameof(GetById), await GetById(orderId, DateTime.Now));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Update(long id, OrderPatchDTO orderPatchDTO)
        {
            await _bll.Orders.UpdateAsync(id, orderPatchDTO);
            return NoContent();
        }

        [HttpPatch("{id}/completion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> UpdateCompletion(long id, OrderCompletionPatchDTO orderCompletionPatchDTO)
        {
            await _bll.Orders.UpdateCompletionAsync(id, orderCompletionPatchDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Delete(long id)
        {
            await _bll.Orders.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/restore")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<IActionResult> Restore(long id)
        {
            await _bll.Orders.RestoreAsync(id);
            return NoContent();
        }
    }
}