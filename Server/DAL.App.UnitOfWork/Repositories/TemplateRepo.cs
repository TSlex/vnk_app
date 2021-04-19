using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;

namespace DAL.App.UnitOfWork.Repositories
{
    public class TemplateRepo: BaseRepo<Entities.Template, Template, AppDbContext>, ITemplateRepo
    {
        public TemplateRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}