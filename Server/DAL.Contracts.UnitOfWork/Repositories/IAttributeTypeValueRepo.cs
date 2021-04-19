using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeTypeValueRepo : IBaseRepo<DAL.App.Entities.AttributeTypeValue, AttributeTypeValue>
    {
        Task<bool> AnyAsync(long valueId, long typeId);
    }
}