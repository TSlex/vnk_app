using BLL.Contracts.Services;

namespace BLL.Contracts
{
    public interface IAppBLL
    {
        IAttributeService Attributes { get; }
        IAttributeTypeService AttributeTypes { get; }
        IAttributeTypeValueService AttributeTypeValues { get; }
        IAttributeTypeUnitService AttributeTypeUnits { get; }
        IOrderService Orders { get; }
        IOrderAttributeService OrderAttributes { get; }
        ITemplateService Templates { get; }
        ITemplateAttributeService TemplateAttributes { get; }
    }
}