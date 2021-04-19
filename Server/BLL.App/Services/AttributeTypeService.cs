using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class AttributeTypeService : BaseService<IAppUnitOfWork>, IAttributeTypeService
    {
        public AttributeTypeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}