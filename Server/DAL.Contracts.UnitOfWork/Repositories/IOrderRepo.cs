using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IOrderRepo : IBaseRepo<DAL.App.Entities.Order, Order>
    {
        Task<IEnumerable<Order>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey,
            DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDateTime);

        Task<int> CountAsync(bool? hasExecutionDate, bool? completed, bool? overdued, string? searchKey,
            DateTime? startDateTime,
            DateTime? endDateTime, DateTime? checkDateTime);

        Task<Order> GetByIdAsync(long id);

        Task<IEnumerable<Order>> GetHistoryAsync(long id, int pageIndex, int itemsOnPage);
        Task<long> CountHistoryAsync(long id);
    }
}