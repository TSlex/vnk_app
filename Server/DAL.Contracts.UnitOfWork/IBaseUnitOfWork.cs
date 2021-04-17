using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}