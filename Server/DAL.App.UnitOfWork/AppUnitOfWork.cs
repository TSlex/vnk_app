using DAL.App.EF;
using DAL.App.Mapper;
using DAL.App.UnitOfWork.Repositories;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork
{
    public class AppUnitOfWork : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
    {
        public AppUnitOfWork(AppDbContext dbContext) : base(dbContext, new UniversalMapper())
        {
        }

        public IOrderRepo Orders => GetRepository<IOrderRepo>(() => new OrderRepo(DbContext, Mapper));
    }
}