using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeValueRepo: BaseRepo<Entities.AttributeTypeValue, AttributeTypeValue, AppDbContext>, IAttributeTypeValueRepo
    {
        public AttributeTypeValueRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}