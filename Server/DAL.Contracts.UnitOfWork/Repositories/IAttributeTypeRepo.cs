using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeTypeRepo : IBaseRepo<DAL.App.Entities.AttributeType, AttributeType>
    {
        Task<IEnumerable<AttributeType>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey);
        Task<long> CountAsync(string? searchKey);
        Task<AttributeType> GetDetailsByIdAsync(long id, int valuesCount, int unitsCount);
        Task<AttributeType> GetWithValuesAndUnits(long attributeTypeId);
    }
}