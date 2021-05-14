using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IOrderAttributeRepo: IBaseRepo<DAL.App.Entities.OrderAttribute, OrderAttribute>
    {
        Task<IEnumerable<OrderAttribute>> GetAllByOrderId(long orderId);
        Task<IEnumerable<OrderAttribute>> GetAllByAttributeIdAsync(long attributeId);
        Task<IEnumerable<OrderAttribute>> GetAllByValueId(long valueId);
        Task<IEnumerable<OrderAttribute>> GetAllByUnitId(long unitId);

        Task<bool> AnyAsync(long id, long orderId);
        Task<int> CountByOrderIdAsync(long id);
    }
}