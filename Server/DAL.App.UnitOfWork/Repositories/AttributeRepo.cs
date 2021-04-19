using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeRepo : BaseRepo<Entities.Attribute, Attribute, AppDbContext>, IAttributeRepo
    {
        public AttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Attribute> GetByIdWithType(long attributeId)
        {
            return MapToDTO(
                await DbSet
                    .Include(a => a.AttributeType)
                    .FirstOrDefaultAsync(a => a.Id == attributeId)
            );
        }
    }
}