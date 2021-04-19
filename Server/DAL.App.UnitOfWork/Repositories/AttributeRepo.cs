using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeRepo: BaseRepo<Entities.Attribute, DTO.Attribute, AppDbContext>, IAttributeRepo
    {
        public AttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}