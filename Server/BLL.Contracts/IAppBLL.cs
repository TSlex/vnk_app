using BLL.Contracts.Services;

namespace BLL.Contracts
{
    public interface IAppBLL
    {
        IOrderService Orders { get; }
    }
}