using BLL.App;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class TemplateService : BaseService<IAppUnitOfWork>, ITemplateService
    {
        public TemplateService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}