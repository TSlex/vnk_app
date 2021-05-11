using BLL.Base.Exceptions;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.Base
{
    public class BaseService<TUnitOfWork> : IBaseService
        where TUnitOfWork : IAppUnitOfWork
    {
        protected BaseService(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected TUnitOfWork UnitOfWork { get; set; }

        protected void ValidationFailed(string message) => ExceptionExecutors.ValidationFailed(message);
        protected void NotFound(string message) => ExceptionExecutors.NotFound(message);
    }
}