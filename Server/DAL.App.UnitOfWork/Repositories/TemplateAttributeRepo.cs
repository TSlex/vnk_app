using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class TemplateAttributeRepo: BaseRepo<Entities.TemplateAttribute, TemplateAttribute, AppDbContext>, ITemplateAttributeRepo
    {
        public TemplateAttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<int> CountByTemplateId(long templateId)
        {
            return await DbSet.CountAsync(ta => ta.TemplateId == templateId);
        }

        public async Task<bool> AnyAsync(long id, long templateId)
        {
            return await DbSet.AnyAsync(ta => ta.Id == id && ta.TemplateId == templateId);
        }

        public async Task<IEnumerable<TemplateAttribute>> GetAllByTemplateId(long id)
        {
            return (await DbSet.Where(ta => ta.TemplateId == id).ToListAsync()).Select(MapToDTO);
        }
    }
}