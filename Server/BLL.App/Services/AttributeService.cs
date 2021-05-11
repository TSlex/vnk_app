using BLL.Base;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class AttributeService : BaseService<IAppUnitOfWork>, IAttributeService
    {
        public AttributeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}