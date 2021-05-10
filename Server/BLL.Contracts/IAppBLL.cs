using BLL.Contracts.Services;

namespace BLL.Contracts
{
    public interface IAppBLL
    {
        IAttributeService Attributes { get; }
        IAttributeTypeService AttributeTypes { get; }
        IOrderService Orders { get; }
        ITemplateService Templates { get; }
        IIdentityService Identity { get; }
    }
}