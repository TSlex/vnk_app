using DAL.Base.UnitOfWork.Repositories;

namespace DAL.Contracts
{
    public interface IAppUnitOfWork : IBaseUnitOfWork
    {
        IAttributeRepo Attributes { get; }
        IAttributeTypeRepo AttributeTypes { get; }
        IAttributeTypeValueRepo AttributeTypeValues { get; }
        IAttributeTypeUnitRepo AttributeTypeUnits { get; }
        IOrderRepo Orders { get; }
        IOrderAttributeRepo OrderAttributes { get; }
        ITemplateRepo Templates { get; }
        ITemplateAttributeRepo TemplateAttributes { get; }
    }
}