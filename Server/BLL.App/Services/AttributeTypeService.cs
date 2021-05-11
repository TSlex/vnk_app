using BLL.Base;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class AttributeTypeService : BaseService<IAppUnitOfWork>, IAttributeTypeService
    {
        public AttributeTypeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}