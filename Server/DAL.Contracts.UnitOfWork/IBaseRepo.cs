using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseRepo
    {
        Task<bool> AnyAsync(long id);
    }
}