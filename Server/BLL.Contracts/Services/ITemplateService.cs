using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;

namespace BLL.Contracts.Services
{
    public interface ITemplateService: IBaseService
    {
        Task<CollectionDTO<TemplateGetDTO>?> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey);
        Task<TemplateGetDTO?> GetByIdAsync(long id);
        Task<long> CreateAsync(TemplatePostDTO templatePostDTO);
        Task UpdateAsync(long id, TemplatePatchDTO templatePatchDTO);
        Task DeleteAsync(long id);
    }
}