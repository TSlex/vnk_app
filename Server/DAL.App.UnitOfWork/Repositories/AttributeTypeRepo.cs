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
    public class AttributeTypeRepo : BaseRepo<Entities.AttributeType, AttributeType, AppDbContext>, IAttributeTypeRepo
    {
        public AttributeTypeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<AttributeType>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName,
            string? searchKey)
        {
            var query = GetActualDataAsQueryable();

            query = query.WhereSuidConditions(searchKey);
            
            query = query.OrderBy(at => at.Id);

            query = byName switch
            {
                SortOption.True => query.OrderBy(at => at.Name),
                SortOption.Reversed => query.OrderByDescending(at => at.Name),
                _ => query
            };

            query = query.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            var attributeTypes = await query.Select(at => new AttributeType
            {
                Id = at.Id,
                Name = at.Name,
                DataType = (AttributeDataType) at.DataType,
                SystemicType = at.SystemicType,
                Attributes = at.Attributes!
                    .Where(a => a.DeletedAt == null)
                    .Select(a => new Attribute())
                    .ToList(),
                UsesDefinedUnits = at.UsesDefinedUnits,
                UsesDefinedValues = at.UsesDefinedValues
            }).ToListAsync();

            return attributeTypes;
        }

        public async Task<long> CountAsync(string? searchKey)
        {
            var query = GetActualDataAsQueryable();

            return await query.WhereSuidConditions(searchKey).CountAsync();
        }

        public async Task<AttributeType> GetDetailsByIdAsync(long id, int valuesCount, int unitsCount)
        {
            var query = GetActualDataByIdAsQueryable(id);

            var attributeType = await query.Select(at => new AttributeType
            {
                Id = at.Id,
                Name = at.Name,
                TypeUnits = at.TypeUnits!
                    .Where(u => u.DeletedAt == null)
                    .Select(u => new AttributeTypeUnit
                    {
                        Id = u.Id,
                        Value = u.Value
                    })
                    .OrderBy(v => v.Id)
                    .Take(unitsCount)
                    .ToList(),
                TypeValues = at.TypeValues!
                    .Where(v => v.DeletedAt == null)
                    .Select(v => new AttributeTypeValue
                    {
                        Id = v.Id,
                        Value = v.Value
                    })
                    .OrderBy(v => v.Id)
                    .Take(valuesCount)
                    .ToList(),
                DataType = (AttributeDataType) at.DataType,
                SystemicType = at.SystemicType,
                Attributes = at.Attributes!
                    .Where(a => a.DeletedAt == null)
                    .Select(a => new Attribute())
                    .ToList(),
                DefaultCustomValue = at.DefaultCustomValue ?? "???",
                DefaultUnitId = at.DefaultUnitId,
                DefaultValueId = at.DefaultValueId,
                UsesDefinedUnits = at.UsesDefinedUnits,
                UsesDefinedValues = at.UsesDefinedValues
            }).FirstOrDefaultAsync();

            return attributeType;
        }

        public async Task<AttributeType> GetWithValuesAndUnits(long attributeTypeId)
        {
            return Mapper.Map<Entities.AttributeType, AttributeType>(await GetActualDataByIdAsQueryable(attributeTypeId)
                .AsNoTracking()
                .Include(at => at.TypeValues!
                    .Where(v => v.DeletedAt == null)
                    .OrderBy(v => v.Id))
                .Include(at => at.TypeUnits!
                    .Where(u => u.DeletedAt == null)
                    .OrderBy(u => u.Id))
                .FirstOrDefaultAsync());
        }
    }

    internal static partial class Extensions
    {
        internal static IQueryable<Entities.AttributeType> WhereSuidConditions(
            this IQueryable<Entities.AttributeType> query,
            string? searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(at => at.Name.ToLower().Contains(searchKey.ToLower()));
            }

            return query;
        }
    }
}