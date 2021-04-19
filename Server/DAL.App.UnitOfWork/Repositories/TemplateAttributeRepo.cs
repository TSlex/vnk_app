using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class TemplateAttributeRepo: BaseRepo<Entities.TemplateAttribute, TemplateAttribute, AppDbContext>, ITemplateAttributeRepo
    {
        public TemplateAttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}