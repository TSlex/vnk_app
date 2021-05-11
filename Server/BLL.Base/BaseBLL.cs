using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Contracts;
using DAL.Contracts;

namespace BLL.Base
{
    public class BaseBLL<TUnitOfWork>: IBaseBLL
        where TUnitOfWork : IBaseUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        private readonly Dictionary<Type, object> _serviceInstances = new Dictionary<Type, object>();

        public BaseBLL(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        
        public TService GetService<TService>(Func<TService> serviceCreationMethod)
        {
            if (_serviceInstances.TryGetValue(typeof (TService), out var obj1))
                return (TService) obj1;
            
            object obj2 = serviceCreationMethod()!;
            
            _serviceInstances.Add(typeof (TService), obj2);
            return (TService) obj2;
        }
        
        public async Task<int> SaveChangesAsync() => await this.UnitOfWork.SaveChangesAsync();

        public int SaveChanges() => this.UnitOfWork.SaveChanges();
    }
}