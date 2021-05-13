using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.DTO.Enums;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeRepo: BaseRepo<Entities.AttributeType, AttributeType, AppDbContext>, IAttributeTypeRepo
    {
        public AttributeTypeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<AttributeType>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey)
        {
            var query = GetActualDataAsQueryable()
                .Include(at => at.Attributes!.Count(a => a.DeletedAt == null))
                .AsQueryable();

            query = query.WhereSuidConditions(searchKey);

            query = byName switch
            {
                SortOption.True => query.OrderBy(at => at.Name),
                SortOption.Reversed => query.OrderByDescending(at => at.Name),
                _ => query
            };

            query = query.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            return (await query.ToListAsync()).Select(Mapper.Map<Entities.AttributeType, AttributeType>);
        }

        public async Task<long> CountAsync(string? searchKey)
        {
            var query = GetActualDataAsQueryable();

            return await query.WhereSuidConditions(searchKey).CountAsync();
        }

        public async Task<AttributeType> GetDetailsByIdAsync(long id, int valuesCount, int unitsCount)
        {
            var query = GetActualDataByIdAsQueryable(id)
                .Include(at => at.TypeUnits!.Where(u => u.DeletedAt == null).Take(unitsCount))
                .Include(at => at.TypeValues!.Where(v => v.DeletedAt == null).Take(valuesCount))
                .Include(at => at.Attributes!.Count(a => a.DeletedAt == null));

            return Mapper.Map<Entities.AttributeType, AttributeType>(await query.FirstOrDefaultAsync());
        }
    }
    
    internal static partial class Extensions
    {
        internal static IQueryable<Entities.AttributeType> WhereSuidConditions(this IQueryable<Entities.AttributeType> query,
            string? searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(at => at.Name.ToLower().Contains(searchKey.ToLower()));
            }

            query = query.OrderBy(at => at.Id);

            return query;
        }
    }
}