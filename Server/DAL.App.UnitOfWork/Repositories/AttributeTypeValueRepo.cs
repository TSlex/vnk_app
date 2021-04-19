﻿using System;
using System.Threading.Tasks;
using DAL.App.DTO;
using DAL.App.EF;
using DAL.Base.UnitOfWork;
using DAL.Base.UnitOfWork.Repositories;
using DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.UnitOfWork.Repositories
{
    public class AttributeTypeValueRepo : BaseRepo<Entities.AttributeTypeValue, AttributeTypeValue, AppDbContext>,
        IAttributeTypeValueRepo
    {
        public AttributeTypeValueRepo(AppDbContext dbContext, IUniversalMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<bool> AnyAsync(long valueId, long typeId)
        {
            return await DbSet.AnyAsync(value => value.Id == valueId && value.AttributeTypeId == typeId);
        }
    }
}