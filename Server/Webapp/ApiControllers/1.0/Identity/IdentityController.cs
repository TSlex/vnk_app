using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.EF;
using Domain;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PublicApi.v1.Common;
using PublicApi.v1.Identity;
using WebApp.Helpers;

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
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public IdentityController(IConfiguration configuration, ILogger<IdentityController> logger,
            UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
            AppDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogInformation($"Web-Api login. AppUser with credentials: {model.Email} - was not found!");
                return BadRequest(new ErrorResponseDTO {Error = "Данные для входа неверны"});
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

                var jwt = JWTGenerator.GenerateJWT(claimsPrincipal.Claims,
                    _configuration["JWT:SigningKey"],
                    _configuration["JWT:Issuer"], 
                    _configuration["JWT:audience"],
                    _configuration.GetValue<int>("JWT:ExpirationInDays")
                );

                _logger.LogInformation($"Token generated for user {model.Email}");
                return Ok(new ResponseDTO<string> {Data = jwt});
            }

            _logger.LogInformation(
                $"Web-Api login. AppUser with credentials: {model.Email} - was attempted to log-in with bad password!");

            return BadRequest(new ErrorResponseDTO {Error = "Данные для входа неверны"});
        }

        [HttpGet("users")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<IEnumerable<UserGetDTO>>))]
        public async Task<ActionResult> GetAllUsers()
        {
            var result = new List<UserGetDTO>();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roleName = "";
                var roleLocalized = "";

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count > 0)
                {
                    roleName = roles[0];

                    roleLocalized = (await _context.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roles[0])))
                        .LocalizedName;
                }

                result.Add(new UserGetDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roleName,
                    RoleLocalized = roleLocalized,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            return Ok(new ResponseDTO<IEnumerable<UserGetDTO>>
            {
                Data = result
            });
        }

        [HttpGet("users/current")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserGetDTO>))]
        public async Task<ActionResult> GetCurrentUser()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var roleName = "";
            var roleLocalized = "";

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                roleName = roles[0];

                roleLocalized = (await _context.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roles[0])))
                    .LocalizedName;
            }

            return Ok(new ResponseDTO<UserGetDTO>
            {
                Data = new UserGetDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roleName,
                    RoleLocalized = roleLocalized,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            });
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO<UserGetDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetUserById(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new ErrorResponseDTO {Error = "Пользователь не найден"});
            }

            var roleName = "";
            var roleLocalized = "";

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                roleName = roles[0];

                roleLocalized = (await _context.Roles.FirstOrDefaultAsync(role => role.Name.Equals(roles[0])))
                    .LocalizedName;
            }

            return Ok(new ResponseDTO<UserGetDTO>
            {
                Data = new UserGetDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roleName,
                    RoleLocalized = roleLocalized,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            });
        }

        [HttpPost("users")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> AddUser([FromBody] UserPostDTO model)
        {
            var appUser = await _userManager.FindByEmailAsync(model.Email);
            if (appUser != null)
            {
                _logger.LogInformation($"WebApi register. User {model.Email} already registered!");
                return BadRequest(new ErrorResponseDTO {Error = "Такой пользователь уже существует"});
            }

            if (ModelState.IsValid)
            {
                var user = new AppUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                AppRole role;

                if (model.Role != null)
                {
                    role = await _roleManager.FindByNameAsync(model.Role);

                    if (role == null)
                    {
                        return BadRequest(new ErrorResponseDTO("Роль не найдена"));
                    }

                    if (!await UserHasAccessToRole(await _userManager.GetUserAsync(User), model.Role))
                    {
                        return BadRequest(new ErrorResponseDTO("Ошибка доступа"));
                    }
                }
                else
                {
                    role = await _roleManager.FindByNameAsync("User");
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Created a new account with password.");

                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else
                    {
                        _logger.LogWarning("User role - \"User\" was not found!");
                    }

                    return Ok();
                }
            }

            return BadRequest(new ErrorResponseDTO {Error = "Ошибка при создании пользователя"});
        }

        [HttpPatch("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUser(long id, [FromBody] UserPatchDTO model)
        {
            if (id != model.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                return NotFound(new ErrorResponseDTO("Пользователь не найден"));
            }

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                return BadRequest(new ErrorResponseDTO("Ошибка доступа"));
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.SetEmailAsync(user, model.Email);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Данный эл.адрес уже занят"));
            }

            result = await _userManager.SetUserNameAsync(user, model.Email);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Данный эл.адрес уже занят"));
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, code);

            return Ok();
        }

        [HttpPatch("users/{id}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUserPassword(long id, [FromBody] UserPasswordPatchDTO model)
        {
            if (id != model.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                return NotFound(new ErrorResponseDTO("Пользователь не найден"));
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                return BadRequest(new ErrorResponseDTO("Ошибка доступа"));
            }

            if (await _userManager.IsInRoleAsync(currentUser, "User") || currentUser.Id == user.Id)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new ErrorResponseDTO("Неверный пароль"));
                }

                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User changed their password successfully.");

                return Ok();
            }

            var passwordValidator = new PasswordValidator<AppUser>();
            var valid = await passwordValidator.ValidateAsync(_userManager, null!, model.NewPassword);

            if (!valid.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Неверный пароль"));
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.NewPassword);

            return Ok();
        }


        [HttpPatch("users/{id}/role")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUserRole(long id, [FromBody] UserRolePatchDTO model)
        {
            if (id != model.Id)
            {
                return BadRequest(new ErrorResponseDTO("Идентификаторы должны совпадать"));
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                return NotFound(new ErrorResponseDTO("Пользователь не найден"));
            }

            var role = await _roleManager.FindByNameAsync(model.Role);

            if (role == null)
            {
                return BadRequest(new ErrorResponseDTO("Роль не найдена"));
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!await UserHasAccessToUser(currentUser, user) || !await UserHasAccessToRole(currentUser, model.Role))
            {
                return BadRequest(new ErrorResponseDTO("Ошибка доступа"));
            }

            var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Ошибка при смене роли"));
            }

            result = await _userManager.AddToRoleAsync(user, model.Role);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Ошибка при смене роли"));
            }

            return Ok();
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Administrator, Root")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(long id)
        {
            if (User.UserId() == id)
            {
                return BadRequest(new ErrorResponseDTO("Вы не можете удалить себя"));
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new ErrorResponseDTO("Пользователь не найден"));
            }

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                return BadRequest(new ErrorResponseDTO("Ошибка доступа"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Ошибка при удалении пользователя"));
            }

            result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Ошибка при удалении пользователя"));
            }

            return Ok();
        }

        private async Task<bool> UserHasAccessToRole(AppUser currentUser, string role)
        {
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            return role switch
            {
                "User" => true,
                "Administrator" => currentUserRoles.Contains("Root"),
                "Root" => false,
                _ => false
            };
        }

        private async Task<bool> UserHasAccessToUser(AppUser currentUser, AppUser targetUser)
        {
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            if (targetUserRoles.Contains("Root"))
            {
                return false;
            }

            if (currentUser.Id == targetUser.Id)
            {
                return true;
            }

            if (targetUserRoles.Contains("Administrator") && currentUserRoles.Contains("Root"))
            {
                return true;
            }

            if (targetUserRoles.Contains("User") &&
                (currentUserRoles.Contains("Administrator") || currentUserRoles.Contains("Root")))
            {
                return true;
            }

            return false;
        }
    }
}