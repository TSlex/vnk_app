using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class AttributeTypeValueService : BaseService<IAppUnitOfWork>, IAttributeTypeValueService
    {
        public AttributeTypeValueService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}