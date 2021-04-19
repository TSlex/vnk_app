using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class AttributeTypeUnitService : BaseService<IAppUnitOfWork>, IAttributeTypeUnitService
    {
        public AttributeTypeUnitService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}