using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class OrderAttributeRepo: BaseRepo<Entities.OrderAttribute, OrderAttribute, AppDbContext>, IOrderAttributeRepo
    {
        public OrderAttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}