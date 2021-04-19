using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class OrderAttributeService : BaseService<IAppUnitOfWork>, IOrderAttributeService
    {
        public OrderAttributeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}