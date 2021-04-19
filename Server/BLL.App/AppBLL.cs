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

        public IAttributeTypeValueService AttributeTypeValues =>
            GetService<IAttributeTypeValueService>(() => new AttributeTypeValueService(UnitOfWork));

        public IAttributeTypeUnitService AttributeTypeUnits =>
            GetService<IAttributeTypeUnitService>(() => new AttributeTypeUnitService(UnitOfWork));

        public IOrderService Orders => GetService<IOrderService>(() => new OrderService(UnitOfWork));

        public IOrderAttributeService OrderAttributes =>
            GetService<IOrderAttributeService>(() => new OrderAttributeService(UnitOfWork));

        public ITemplateService Templates => GetService<ITemplateService>(() => new TemplateService(UnitOfWork));

        public ITemplateAttributeService TemplateAttributes =>
            GetService<ITemplateAttributeService>(() => new TemplateAttributeService(UnitOfWork));
    }
}