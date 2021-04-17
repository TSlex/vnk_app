using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork
{
    public class BaseRepo<TDbContext>: IBaseRepo
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;

        public BaseRepo(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}