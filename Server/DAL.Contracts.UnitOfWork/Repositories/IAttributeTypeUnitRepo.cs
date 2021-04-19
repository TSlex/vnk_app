using System.Threading.Tasks;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeTypeUnitRepo : IBaseRepo
    {
        Task<bool> AnyAsync(long unitId, long typeId);
    }
}