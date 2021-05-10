using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPI._1._0.Identity;

namespace BLL.Contracts.Services
{
    public interface IIdentityService
    {
        Task<string> LoginUserAsync(LoginDTO loginDTO);
        Task<IEnumerable<UserGetDTO>> GetAllAsync();
        Task<UserGetDTO> GetByIdAsync(long userId);
        Task CreateAsync(UserPostDTO userPostDTO);
        Task UpdateAsync(long id, UserPatchDTO userPatchDTO);
        Task UpdatePasswordAsync(long id, UserPasswordPatchDTO userPasswordPatchDTO);
        Task UpdateRoleAsync(long id, UserRolePatchDTO userRolePatchDTO);
        Task DeleteAsync(long id);
    }
}