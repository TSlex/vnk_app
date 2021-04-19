using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseUnitOfWork
    {
        IUniversalMapper Mapper { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}