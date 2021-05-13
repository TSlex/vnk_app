using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;

namespace BLL.Contracts.Services
{
    public interface IAttributeTypeService: IBaseService
    {
        Task<CollectionDTO<AttributeTypeGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey);
        Task<AttributeTypeDetailsGetDTO> GetByIdAsync(long id, int valuesCount, int unitsCount);
        Task<long> CreateAsync(AttributeTypePostDTO attributeTypePostDTO);
        Task UpdateAsync(long id, AttributeTypePatchDTO attributeTypePatchDTO);
        Task DeleteAsync(long id);
    }
}