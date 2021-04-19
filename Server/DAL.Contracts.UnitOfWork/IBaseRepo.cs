using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IBaseRepo
    {
        Task<bool> ExistsAsync(long id);
    }
}