using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;

namespace DAL.App.UnitOfWork.Repositories
{
    public class OrderRepository: BaseRepo<AppDbContext>, IOrderRepository
    {
        public OrderRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}