﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class OrderAttributeRepo: BaseRepo<Entities.OrderAttribute, OrderAttribute, AppDbContext>, IOrderAttributeRepo
    {
        public OrderAttributeRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<OrderAttribute>> GetAllByOrderId(long id)
        {
            return (await DbSet.Where(ta => ta.OrderId == id).ToListAsync()).Select(MapToDTO);
        }

        public void RemoveRange(IEnumerable<OrderAttribute> orderAttributes)
        {
            DbSet.RemoveRange(orderAttributes.Select(MapToEntity));
        }
    }
}