using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;

namespace BLL.Contracts.Services
{
    public interface IAttributeService : IBaseService
    {
        Task<CollectionDTO<AttributeGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName,
            SortOption byType, string? searchKey);

        Task<AttributeDetailsGetDTO> GetByIdAsync(long id);
        Task<long> CreateAsync(AttributePostDTO attributePostDTO);
        Task UpdateAsync(long id, AttributePatchDTO attributePatchDTO);
        Task DeleteAsync(long id);
    }
}