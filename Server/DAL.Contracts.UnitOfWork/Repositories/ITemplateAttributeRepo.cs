using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface ITemplateAttributeRepo: IBaseRepo<DAL.App.Entities.TemplateAttribute, TemplateAttribute>
    {
        Task<int> CountByTemplateId(long id);
        Task<bool> AnyAsync(long id, long templateId);
        Task<IEnumerable<TemplateAttribute>> GetAllByTemplateId(long id);
    }
}