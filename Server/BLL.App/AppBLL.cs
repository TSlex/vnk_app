using BLL.App.Services;
using BLL.Contracts;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App
{
    public class AppBLL : BaseBLL<IAppUnitOfWork>, IAppBLL
    {
        public AppBLL(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IAttributeService Attributes => GetService<IAttributeService>(() => new AttributeService(UnitOfWork));

        public IAttributeTypeService AttributeTypes =>
            GetService<IAttributeTypeService>(() => new AttributeTypeService(UnitOfWork));

        public IOrderService Orders => GetService<IOrderService>(() => new OrderService(UnitOfWork));

        public ITemplateService Templates => GetService<ITemplateService>(() => new TemplateService(UnitOfWork));

        public IIdentityService Identity => GetService<IIdentityService>(() => new IdentityService());
    }
}