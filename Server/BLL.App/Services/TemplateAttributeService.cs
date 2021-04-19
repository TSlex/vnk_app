using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class TemplateAttributeService : BaseService<IAppUnitOfWork>, ITemplateAttributeService
    {
        public TemplateAttributeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}