using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeRepo: BaseRepo<Entities.AttributeType, AttributeType, AppDbContext>, IAttributeTypeRepo
    {
        public AttributeTypeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}