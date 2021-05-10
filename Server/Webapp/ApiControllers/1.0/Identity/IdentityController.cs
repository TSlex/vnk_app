using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPI._1._0.Identity;
using AppAPI._1._0.Responses;
using BLL.App.Exceptions;
using BLL.Contracts;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapp.ApiControllers._1._0.Identity
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "User, Administrator, Root")]
    public class IdentityController : ControllerBase
    {
        private readonly IAppBLL _bll;

        public IdentityController(IAppBLL bll)
        {
            _bll = bll;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var jwt = await _bll.Identity.LoginUserAsync(loginDTO);

                return Ok(new ResponseDTO<string> {Data = jwt});
            }
            catch (ValidationException notFoundException)
            {
                return BadRequest(new ErrorResponseDTO(notFoundException.Message));
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<IEnumerable<UserGetDTO>>))]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _bll.Identity.GetAllAsync();

            return Ok(new ResponseDTO<IEnumerable<UserGetDTO>>
            {
                Data = users
            });
        }

        [HttpGet("users/current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetCurrentUser()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            try
            {
                var user = await _bll.Identity.GetByIdAsync(User!.UserId());

                return Ok(new ResponseDTO<UserGetDTO> {Data = user});
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> GetUserById(long id)
        {
            try
            {
                var user = await _bll.Identity.GetByIdAsync(id);

                return Ok(new ResponseDTO<UserGetDTO> {Data = user});
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
        }

        [HttpPost("users")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult<string>> AddUser([FromBody] UserPostDTO userPostDTO)
        {
            try
            {
                await _bll.Identity.CreateAsync(userPostDTO);
                return Ok();
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorResponseDTO(validationException.Message));
            }
        }

        [HttpPatch("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateUser(long id, [FromBody] UserPatchDTO userPatchDTO)
        {
            try
            {
                await _bll.Identity.UpdateAsync(id, userPatchDTO);
                return Ok();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorResponseDTO(validationException.Message));
            }
        }

        [HttpPatch("users/{id}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateUserPassword(long id,
            [FromBody] UserPasswordPatchDTO userPasswordPatchDTO)
        {
            try
            {
                await _bll.Identity.UpdatePasswordAsync(id, userPasswordPatchDTO);
                return Ok();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorResponseDTO(validationException.Message));
            }
        }


        [HttpPatch("users/{id}/role")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> UpdateUserRole(long id, [FromBody] UserRolePatchDTO userRolePatchDTO)
        {
            try
            {
                await _bll.Identity.UpdateRoleAsync(id, userRolePatchDTO);
                return Ok();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorResponseDTO(validationException.Message));
            }
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> DeleteUser(long id)
        {
            try
            {
                await _bll.Identity.DeleteAsync(id);
                return Ok();
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new ErrorResponseDTO(notFoundException.Message));
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorResponseDTO(validationException.Message));
            }
        }
    }
}