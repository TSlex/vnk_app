using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class OrderAttributeRepo : BaseRepo<Entities.OrderAttribute, OrderAttribute, AppDbContext>,
        IOrderAttributeRepo
    {
        public OrderAttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<OrderAttribute>> GetAllByOrderId(long orderId)
        {
            return (await GetActualDataAsQueryable().Where(oa => oa.OrderId == orderId).ToListAsync())
                .Select(MapToDTO);
        }

        public async Task<IEnumerable<OrderAttribute>> GetAllByAttributeIdAsync(long attributeId)
        {
            return (await GetActualDataAsQueryable().Where(oa => oa.AttributeId == attributeId).ToListAsync())
                .Select(MapToDTO);
        }

        public async Task<IEnumerable<OrderAttribute>> GetAllByValueId(long valueId)
        {
            return (await GetActualDataAsQueryable().Where(oa => oa.ValueId == valueId).ToListAsync())
                .Select(MapToDTO);
        }

        public async Task<IEnumerable<OrderAttribute>> GetAllByUnitId(long unitId)
        {
            return (await GetActualDataAsQueryable().Where(oa => oa.UnitId == unitId).ToListAsync())
                .Select(MapToDTO);
        }

        public async Task<bool> AnyAsync(long id, long orderId)
        {
            return await GetActualDataAsQueryable().AnyAsync(oa => oa.Id == id && oa.OrderId == orderId);
        }

        public async Task<int> CountByOrderIdAsync(long orderId)
        {
            return await GetActualDataAsQueryable().CountAsync(oa => oa.OrderId == orderId);
        }
    }
}