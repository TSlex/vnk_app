using System;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;

namespace BLL.Contracts.Services
{
    public interface IOrderService: IBaseService
    {
        Task<CollectionDTO<OrderGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, bool hasExecutionDate, bool? completed, bool? overdued, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDatetime);

        Task<OrderGetDTO> GetByIdAsync(long id, DateTime? checkDatetime);
        
        Task<long> CreateAsync(OrderPostDTO orderPostDTO);
        Task UpdateAsync(long id, OrderPatchDTO orderPatchDTO);
        
        Task DeleteAsync(long id);
        Task UpdateCompletionAsync(long id, OrderCompletionPatchDTO orderCompletionPatchDTO);
        Task RestoreAsync(long id);
        
        Task<CollectionDTO<OrderHistoryGetDTO>> GetHistoryAsync(long id, int pageIndex, int itemsOnPage);
    }
}