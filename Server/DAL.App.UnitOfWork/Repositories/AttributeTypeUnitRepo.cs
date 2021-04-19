using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeUnitRepo : BaseRepo<Entities.AttributeTypeUnit, AttributeTypeUnit, AppDbContext>,
        IAttributeTypeUnitRepo
    {
        public AttributeTypeUnitRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<bool> AnyAsync(long unitId, long typeId)
        {
            return await DbSet.AnyAsync(unit => unit.Id == unitId && unit.AttributeTypeId == typeId);
        }
    }
}