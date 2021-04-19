using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App
{
    public class BaseService<TUnitOfWork> : IBaseService
        where TUnitOfWork : IAppUnitOfWork
    {
        protected BaseService(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected TUnitOfWork UnitOfWork { get; set; }
    }
}