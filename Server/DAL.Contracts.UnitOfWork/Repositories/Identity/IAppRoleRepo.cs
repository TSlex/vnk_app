using System.Threading.Tasks;
using DAL.App.DTO.Identity;

namespace DAL.Base.UnitOfWork.Repositories.Identity
{
    public interface IAppRoleRepo
    {
        Task<AppRole> GetByRoleNameAsync(string roleName);
    }
}