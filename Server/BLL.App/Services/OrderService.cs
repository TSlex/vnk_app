using System;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using BLL.Contracts.Services;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class OrderService: BaseService<IAppUnitOfWork>, IOrderService
    {
        public OrderService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<OrderGetDTO>> GetAll(int pageIndex, int itemsOnPage,
            SortOption byName, bool? completed, string? searchKey, DateTime? startDateTime,
            DateTime? endDateTime, DateTime? overdueDatetime)
        {
        }

        public async Task<OrderGetDTO> GetById(long id)
        {
            
        }

        public async Task<OrderGetDTO> Create(OrderPostDTO orderPostDTO)
        {
            
        }

        public async Task Update(long id, OrderPatchDTO orderPatchDTO)
        {
            
        }
        
        public async Task SetCompleted(long id, OrderCompletionPatchDTO orderPatchDTO)
        {
            
        }

        public async Task Delete(long id)
        {
            
        }
        
        public async Task<CollectionDTO<OrderHistoryGetDTO>> GetHistory(long id)
        {
            
        }
    }
}