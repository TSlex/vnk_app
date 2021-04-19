using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeRepo: IBaseRepo
    {
        Task<Attribute> GetByIdWithType(long attributeId);
    }
}