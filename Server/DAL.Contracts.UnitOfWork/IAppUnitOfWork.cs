using DAL.Base.UnitOfWork.Repositories;

namespace DAL.Contracts
{
    public interface IAppUnitOfWork: IBaseUnitOfWork
    {
        IOrderRepository Orders { get; }
    }
}