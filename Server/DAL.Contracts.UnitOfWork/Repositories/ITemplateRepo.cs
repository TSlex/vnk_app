using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface ITemplateRepo: IBaseRepo<DAL.App.Entities.Template, Template>
    {
        Task<IEnumerable<Template>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey);
        Task<long> CountAsync(string? searchKey);
        Task<Template> GetByIdAsync(long id);
    }
}