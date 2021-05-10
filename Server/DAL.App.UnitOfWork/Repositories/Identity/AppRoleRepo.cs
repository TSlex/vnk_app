using System.Threading.Tasks;
using DAL.App.EF;
using DAL.App.Entities.Identity;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork.Repositories.Identity
{
    public class AppRoleRepo : IAppRoleRepo
    {
        private readonly AppDbContext DbContext;
        private readonly IUniversalMapper Mapper;
        private readonly DbSet<AppRole> DbSet;

        public AppRoleRepo(AppDbContext dbContext, IUniversalMapper mapper)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<AppRole>();
            Mapper = mapper;
        }

        public async Task<App.DTO.Identity.AppRole> GetByRoleNameAsync(string roleName)
        {
            return Mapper.Map<AppRole, App.DTO.Identity.AppRole>(
                await DbSet.FirstOrDefaultAsync(appRole => appRole.Name.Equals(roleName)));
        }
    }
}