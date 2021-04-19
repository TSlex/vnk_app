using DAL.Base.UnitOfWork.Repositories;

namespace DAL.Contracts
{
    public interface IAppUnitOfWork: IBaseUnitOfWork
    {
        IOrderRepo Orders { get; }
    }
}