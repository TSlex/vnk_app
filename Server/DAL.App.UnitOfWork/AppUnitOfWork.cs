using DAL.App.EF;
using DAL.App.UnitOfWork.Repositories;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork
{
    public class AppUnitOfWork : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
    {
        protected AppUnitOfWork(AppDbContext dbContext) : base(dbContext)
        {
        }

        public IOrderRepository Orders => GetRepository<IOrderRepository>(() => new OrderRepository(DbContext));
    }
}