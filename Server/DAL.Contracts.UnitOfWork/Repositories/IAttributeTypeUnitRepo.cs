using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeTypeUnitRepo : IBaseRepo<DAL.App.Entities.AttributeTypeUnit, AttributeTypeUnit>
    {
        Task<bool> AnyAsync(long unitId, long typeId);
    }
}