using BLL.App.Services;
using BLL.Contracts;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App
{
    public class AppBLL: BaseBLL<IAppUnitOfWork>, IAppBLL
    {
        public AppBLL(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public IOrderService Orders => GetService<IOrderService>(() => new OrderService(UnitOfWork));
    }
}