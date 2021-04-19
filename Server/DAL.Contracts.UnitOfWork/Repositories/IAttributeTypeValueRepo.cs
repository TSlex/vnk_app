using System;
using System.Threading.Tasks;
using DAL.Contracts;

namespace DAL.Base.UnitOfWork.Repositories
{
    public interface IAttributeTypeValueRepo : IBaseRepo
    {
        Task<bool> AnyAsync(long valueId, long typeId);
    }
}