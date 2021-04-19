using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.App.Entities;
using DAL.App.Entities.Identity;
using DAL.Contracts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Attribute = DAL.App.Entities.Attribute;

namespace DAL.App.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public DbSet<Attribute> Attributes { get; set; } = default!;
        public DbSet<AttributeType> AttributeTypes { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderAttribute> OrderAttributes { get; set; } = default!;
        public DbSet<Template> Templates { get; set; } = default!;
        public DbSet<TemplateAttribute> TemplateAttributes { get; set; } = default!;
        public DbSet<AttributeTypeValue> TypeValues { get; set; } = default!;
        public DbSet<AttributeTypeUnit> TypeUnits { get; set; } = default!;

        private readonly IUserProvider _userProvider;

        public AppDbContext(DbContextOptions options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b => b.ToTable("AppUser"));
            builder.Entity<AppRole>(b => b.ToTable("AppRole"));

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private void SaveChangesMetadataUpdate()
        {
            ChangeTracker.DetectChanges();

            var addedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            foreach (var entityEntry in addedEntities)
            {
                if (!(entityEntry.Entity is IDomainEntityMetadata entityWithMetaData)) continue;
                if (entityEntry.Entity is IDomainEntitySoftUpdate softUpdateEntity &&
                    softUpdateEntity.MasterId != null) continue;

                entityWithMetaData.CreatedAt = DateTime.UtcNow;
                entityWithMetaData.CreatedBy ??= _userProvider.CurrentName;

                entityWithMetaData.ChangedAt = entityWithMetaData.CreatedAt;
                entityWithMetaData.ChangedBy = entityWithMetaData.CreatedBy;
            }

            var deletedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);

            foreach (var entityEntry in deletedEntities)
            {
                if (entityEntry.Entity is not IDomainEntitySoftDelete softDeleteEntity) continue;

                softDeleteEntity.DeletedAt = DateTime.UtcNow;
                softDeleteEntity.DeletedBy = _userProvider.CurrentName;

                entityEntry.State = EntityState.Modified;
            }

            var updatedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

            foreach (var entityEntry in updatedEntities)
            {
                if (!(entityEntry.Entity is IDomainEntityMetadata entityWithMetaData)) continue;

                entityWithMetaData.ChangedAt = DateTime.UtcNow;
                entityWithMetaData.ChangedBy = _userProvider.CurrentName;

                if (entityEntry.Entity is IDomainEntitySoftUpdate) continue;

                entityEntry.Property(nameof(entityWithMetaData.CreatedAt)).IsModified = false;
                entityEntry.Property(nameof(entityWithMetaData.CreatedBy)).IsModified = false;
            }
        }


        public override int SaveChanges()
        {
            SaveChangesMetadataUpdate();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            SaveChangesMetadataUpdate();
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}