using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppAPI._1._0.Identity;
using BLL.App.Exceptions;
using BLL.Contracts;
using BLL.Contracts.Services;
using DAL.App.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BLL.App.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IAppBLL> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IdentityService(IConfiguration configuration, ILogger<IAppBLL> logger,
            UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<string> LoginUserAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                _logger.LogInformation($"Пользователь, {loginDTO.Email}, не найден!");

                throw new ValidationException("Данные для входа неверны");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (result.Succeeded)
            {
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

                var jwt = GenerateJWT(claimsPrincipal.Claims,
                    _configuration["JWT:SigningKey"],
                    _configuration["JWT:Issuer"],
                    _configuration["JWT:audience"],
                    Convert.ToInt32(_configuration["JWT:ExpirationInDays"])
                );

                _logger.LogInformation($"JWT сгенерирорван для {loginDTO.Email}");
                return jwt;
            }

            _logger.LogInformation(
                $"Пользователь, {loginDTO.Email}, пытался авторизоваться, используя неправильный пароль!");

            throw new ValidationException("Данные для входа неверны");
        }

        public async Task<IEnumerable<UserGetDTO>> GetAllAsync()
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

            return result;
        }

        public async Task<UserGetDTO> GetByIdAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new ValidationException("Пользователь не найден");
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

            return new UserGetDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = roleName,
                RoleLocalized = roleLocalized,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task CreateAsync(UserPostDTO userPostDTO)
        {
            var appUser = await _userManager.FindByEmailAsync(userPostDTO.Email);
            if (appUser != null)
            {
                _logger.LogInformation($"WebApi register. User {userPostDTO.Email} already registered!");
                throw new ValidationException("Такой пользователь уже существует");
            }

            var user = new AppUser()
            {
                UserName = userPostDTO.Email,
                Email = userPostDTO.Email,
                EmailConfirmed = true,
                FirstName = userPostDTO.FirstName,
                LastName = userPostDTO.LastName
            };

            AppRole role;

            if (userPostDTO.Role != null)
            {
                role = await _roleManager.FindByNameAsync(userPostDTO.Role);

                if (role == null)
                {
                    throw new ValidationException("Роль не найдена");
                }

                if (!await UserHasAccessToRole(await _userManager.GetUserAsync(User), userPostDTO.Role))
                {
                    throw new ValidationException("Ошибка доступа");
                }
            }
            else
            {
                role = await _roleManager.FindByNameAsync("User");
            }

            var result = await _userManager.CreateAsync(user, userPostDTO.Password);

            if (!result.Succeeded)
            {
                throw new ValidationException("Ошибка при создании пользователя");
            }

            _logger.LogInformation($"Создан новый пользователь - {user.Email}, с ролью \"{role}\"");

            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
            else
            {
                _logger.LogWarning("User role - \"User\" was not found!");
            }
        }

        public async Task UpdateAsync(long id, UserPatchDTO userPatchDTO)
        {
            if (id != userPatchDTO.Id)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var user = await _userManager.FindByIdAsync(userPatchDTO.Id.ToString());

            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден");
            }

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                throw new ValidationException("Ошибка доступа");
            }

            user.FirstName = userPatchDTO.FirstName;
            user.LastName = userPatchDTO.LastName;

            var result = await _userManager.SetEmailAsync(user, userPatchDTO.Email);

            if (!result.Succeeded)
            {
                throw new ValidationException("Данный эл.адрес уже занят");
            }

            result = await _userManager.SetUserNameAsync(user, userPatchDTO.Email);

            if (!result.Succeeded)
            {
                throw new ValidationException("Данный эл.адрес уже занят");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task UpdatePasswordAsync(long id, UserPasswordPatchDTO userPasswordPatchDTO)
        {
            if (id != userPasswordPatchDTO.Id)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var user = await _userManager.FindByIdAsync(userPasswordPatchDTO.Id.ToString());

            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                throw new ValidationException("Ошибка доступа");
            }

            if (await _userManager.IsInRoleAsync(currentUser, "User") || currentUser.Id == user.Id)
            {
                var result = await _userManager.ChangePasswordAsync(user, userPasswordPatchDTO.CurrentPassword,
                    userPasswordPatchDTO.NewPassword);

                if (!result.Succeeded)
                {
                    throw new ValidationException("Неверный пароль");
                }

                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("User changed their password successfully.");

                return;
            }

            var passwordValidator = new PasswordValidator<AppUser>();
            var valid = await passwordValidator.ValidateAsync(_userManager, null!, userPasswordPatchDTO.NewPassword);

            if (!valid.Succeeded)
            {
                throw new ValidationException("Неверный пароль");
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, userPasswordPatchDTO.NewPassword);
        }

        public async Task UpdateRoleAsync(long id, UserRolePatchDTO userRolePatchDTO)
        {
            if (id != userRolePatchDTO.Id)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var user = await _userManager.FindByIdAsync(userRolePatchDTO.Id.ToString());

            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден");
            }

            var role = await _roleManager.FindByNameAsync(userRolePatchDTO.Role);

            if (role == null)
            {
                throw new ValidationException("Неправильная роль");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!await UserHasAccessToUser(currentUser, user) ||
                !await UserHasAccessToRole(currentUser, userRolePatchDTO.Role))
            {
                throw new ValidationException("Ошибка доступа");
            }

            var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

            if (!result.Succeeded)
            {
                throw new ValidationException("Ошибка при смене роли");
            }

            result = await _userManager.AddToRoleAsync(user, userRolePatchDTO.Role);

            if (!result.Succeeded)
            {
                throw new ValidationException("Ошибка при смене роли");
            }
        }

        public async Task DeleteAsync(long id)
        {
            if (User.UserId() == id)
            {
                throw new ValidationException("Вы не можете удалить себя");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new NotFoundException("Пользователь не найден");
            }

            if (!await UserHasAccessToUser(await _userManager.GetUserAsync(User), user))
            {
                throw new ValidationException("Ошибка доступа");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                throw new ValidationException("Ошибка при удалении пользователя");
            }

            result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new ValidationException("Ошибка при удалении пользователя");
            }
        }

        #region Helpers

        private static string GenerateJWT(IEnumerable<Claim> claims, string signingKey, string issuer, string audience,
            int expiresInDays)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var singingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                null,
                DateTime.UtcNow.AddDays(expiresInDays),
                singingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        #endregion
    }
}