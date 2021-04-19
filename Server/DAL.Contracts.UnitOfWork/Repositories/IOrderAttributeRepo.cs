using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IOrderAttributeRepo: IBaseRepo
    {
        Task<IEnumerable<OrderAttribute>> GetAllByOrderId(long id);
        void RemoveRange(IEnumerable<OrderAttribute> orderAttributes);
    }
}