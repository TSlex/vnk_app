using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.UnitOfWork
{
    public class BaseUnitOfWork<TDbContext> : IBaseUnitOfWork
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;
        public IUniversalMapper Mapper { get; }
        
        private readonly Dictionary<Type, object> _repoInstances = new Dictionary<Type, object>();

        protected BaseUnitOfWork(TDbContext dbContext, IUniversalMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        protected TRepository GetRepository<TRepository>(Func<TRepository> repoCreationMethod)
        {
            if (_repoInstances.TryGetValue(typeof(TRepository), out var obj1))
                return (TRepository) obj1!;

            object obj2 = repoCreationMethod()!;

            _repoInstances.Add(typeof(TRepository), obj2);

            return (TRepository) obj2;
        }

        public int SaveChanges() => DbContext.SaveChanges();

        public async Task<int> SaveChangesAsync() => await DbContext.SaveChangesAsync(new CancellationToken());
    }
}