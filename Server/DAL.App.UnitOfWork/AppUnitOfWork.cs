using DAL.App.EF;
using DAL.App.Mapper;
using DAL.App.UnitOfWork.Repositories;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork
{
    public class AppUnitOfWork : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
    {
        public AppUnitOfWork(AppDbContext dbContext) : base(dbContext, new UniversalMapper())
        {
        }

        public IAttributeRepo Attributes => GetRepository<IAttributeRepo>(
            () => new AttributeRepo(DbContext, Mapper));

        public IAttributeTypeRepo AttributeTypes => GetRepository<IAttributeTypeRepo>(
            () => new AttributeTypeRepo(DbContext, Mapper));

        public IAttributeTypeValueRepo AttributeTypeValues => GetRepository<IAttributeTypeValueRepo>(
            () => new AttributeTypeValueRepo(DbContext, Mapper));

        public IAttributeTypeUnitRepo AttributeTypeUnits => GetRepository<IAttributeTypeUnitRepo>(
            () => new AttributeTypeUnitRepo(DbContext, Mapper));

        public IOrderRepo Orders => GetRepository<IOrderRepo>(
            () => new OrderRepo(DbContext, Mapper));

        public IOrderAttributeRepo OrderAttributes => GetRepository<IOrderAttributeRepo>(
            () => new OrderAttributeRepo(DbContext, Mapper));

        public ITemplateRepo Templates => GetRepository<ITemplateRepo>(
            () => new TemplateRepo(DbContext, Mapper));

        public ITemplateAttributeRepo TemplateAttributes => GetRepository<ITemplateAttributeRepo>(
            () => new TemplateAttributeRepo(DbContext, Mapper));
    }
}