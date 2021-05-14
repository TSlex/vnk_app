using System;
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
    public class TemplateRepo : BaseRepo<Entities.Template, Template, AppDbContext>, ITemplateRepo
    {
        public TemplateRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<Template>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName,
            string? searchKey)
        {
            var query = GetActualDataAsQueryable()
                .IncludeAttributesFull();

            query = query.WhereSuidConditions(searchKey);

            query = query.OrderBy(t => t.Id);

            query = byName switch
            {
                SortOption.True => query.OrderBy(t => t.Name),
                SortOption.Reversed => query.OrderByDescending(t => t.Name),
                _ => query
            };

            query = query.Skip(itemsOnPage * pageIndex).Take(itemsOnPage);

            return (await query.ToListAsync()).Select(Mapper.Map<Entities.Template, Template>);
        }

        public async Task<Template> GetByIdAsync(long id)
        {
            var query = GetActualDataByIdAsQueryable(id)
                .IncludeAttributesFull();

            return Mapper.Map<Entities.Template, Template>(await query.FirstOrDefaultAsync());
        }

        public async Task<long> CountAsync(string? searchKey)
        {
            var query = GetActualDataAsQueryable();

            return await query.WhereSuidConditions(searchKey).CountAsync();
        }
    }

    internal static partial class Extensions
    {
        internal static IQueryable<Entities.Template> WhereSuidConditions(this IQueryable<Entities.Template> query,
            string? searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(t => t.Name.ToLower().Contains(searchKey.ToLower()));
            }

            query = query.OrderBy(t => t.Id);

            return query;
        }

        internal static IQueryable<Entities.Template> IncludeAttributesFull(this IQueryable<Entities.Template> query)
        {
            return query
                .Include(o => o.TemplateAttributes)
                .ThenInclude(oa => oa.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .ThenInclude(t => t!.TypeValues!.Where(v =>
                    v.DeletedAt == null))
                .Include(o => o.TemplateAttributes)
                .ThenInclude(oa => oa.Attribute)
                .ThenInclude(a => a!.AttributeType)
                .ThenInclude(t => t!.TypeUnits!.Where(u =>
                    u.DeletedAt == null));
        }
    }
}