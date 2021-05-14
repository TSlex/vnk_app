using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.Contracts;


namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeRepo : IBaseRepo<DAL.App.Entities.Attribute, Attribute>
    {
        Task<IEnumerable<Attribute>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, SortOption byType,
            string? searchKey);

        Task<Attribute> GetByIdAsync(long id);
        Task<Attribute> GetByIdWithType(long attributeId);

        Task<bool> AnyByTypeIdAsync(long attributeTypeId);
        Task<long> CountAsync(string? searchKey);
    }
}