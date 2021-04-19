using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeUnitRepo: BaseRepo<Entities.AttributeTypeUnit, AttributeTypeUnit, AppDbContext>, IAttributeTypeUnitRepo
    {
        public AttributeTypeUnitRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}