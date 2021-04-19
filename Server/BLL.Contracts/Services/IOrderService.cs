using System;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;

namespace BLL.Contracts.Services
{
    public interface IOrderService
    {
        Task<CollectionDTO<OrderGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, bool hasExecutionDate, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime);

        Task<OrderGetDTO> GetByIdAsync(long id, DateTime? checkDatetime);
        
        Task CreateAsync(OrderPostDTO orderPostDTO);
        Task UpdateAsync(OrderPatchDTO orderPatchDTO);
        
        Task DeleteAsync(long id);
    }
}