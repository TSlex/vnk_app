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
    public class AttributeRepo : BaseRepo<Entities.Attribute, Attribute, AppDbContext>, IAttributeRepo
    {
        public AttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<Attribute>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName,
            SortOption byType, string? searchKey)
        {
            var query = GetActualDataAsQueryable()
                .Include(a => a.AttributeType)
                .AsQueryable();

            query = query.WhereSuidConditions(searchKey);

            query = query.OrderBy(a => a.Id);

            query = byName switch
            {
                SortOption.True => query.OrderBy(a => a.Name),
                SortOption.Reversed => query.OrderByDescending(a => a.Name),
                _ => query
            };

            query = byType switch
            {
                SortOption.True => query.OrderBy(a => a.AttributeType!.Name),
                SortOption.Reversed => query.OrderByDescending(a => a.AttributeType!.Name),
                _ => query
            };

            query = query.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            return (await query.ToListAsync()).Select(Mapper.Map<Entities.Attribute, Attribute>);
        }

        public async Task<Attribute> GetByIdAsync(long id)
        {
            var attribute = await GetActualDataByIdAsQueryable(id)
                .Select(a => new Entities.Attribute
                {
                    Id = a.Id,
                    Name = a.Name,
                    OrderAttributes = a.OrderAttributes!
                        .Where(oa => oa.DeletedAt == null)
                        .Select(oa => new Entities.OrderAttribute())
                        .ToList(),
                    AttributeTypeId = a.AttributeTypeId,
                    AttributeType = new Entities.AttributeType
                    {
                        Name = a.AttributeType!.Name,
                        DataType = a.AttributeType!.DataType,
                        TypeUnits = a.AttributeType!.TypeUnits!
                            .Where(u => u.Id == a.AttributeType!.DefaultUnitId).ToList(),
                        TypeValues = a.AttributeType!.TypeValues!
                            .Where(v => v.Id == a.AttributeType!.DefaultValueId).ToList(),
                        DefaultCustomValue = a.AttributeType!.DefaultCustomValue
                    }
                })
                .FirstOrDefaultAsync();

            return Mapper.Map<Entities.Attribute, Attribute>(attribute);
        }

        public async Task<Attribute> GetByIdWithType(long attributeId)
        {
            return MapToDTO(
                await GetActualDataByIdAsQueryable(attributeId)
                    .Include(a => a.AttributeType).FirstOrDefaultAsync()
            );
        }

        public async Task<bool> AnyByTypeIdAsync(long attributeTypeId)
        {
            return await GetActualDataAsQueryable()
                .AnyAsync(a => a.AttributeTypeId == attributeTypeId && a.DeletedAt == null);
        }

        public async Task<long> CountAsync(string? searchKey)
        {
            var query = GetActualDataAsQueryable();

            return await query.WhereSuidConditions(searchKey).CountAsync();
        }
    }

    internal static partial class Extensions
    {
        internal static IQueryable<Entities.Attribute> WhereSuidConditions(this IQueryable<Entities.Attribute> query,
            string? searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(a => a.Name.ToLower().Contains(searchKey.ToLower()));
            }
            
            return query;
        }
    }
}