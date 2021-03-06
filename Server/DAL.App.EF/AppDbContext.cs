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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }

        private void SaveChangesMetadataUpdate()
        {
            ChangeTracker.DetectChanges();
            var now = DateTime.UtcNow;
            var currentUserName = $"{_userProvider.CurrentId}:{_userProvider.CurrentName}";
            currentUserName = _userProvider.CurrentId.Length > 0 ? currentUserName : "";

            var addedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            foreach (var entityEntry in addedEntities)
            {
                if (!(entityEntry.Entity is IDomainEntityMetadata entityWithMetaData)) continue;

                if (entityEntry.Entity is IDomainEntitySoftUpdate softUpdateEntity &&
                    softUpdateEntity.MasterId != null)
                {
                    softUpdateEntity.DeletedAt = now;
                    softUpdateEntity.DeletedBy = currentUserName;
                    continue;
                }

                entityWithMetaData.CreatedAt = now;
                entityWithMetaData.CreatedBy = currentUserName;

                entityWithMetaData.ChangedAt = entityWithMetaData.CreatedAt;
                entityWithMetaData.ChangedBy = entityWithMetaData.CreatedBy;
            }

            var updatedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

            foreach (var entityEntry in updatedEntities)
            {
                if (!(entityEntry.Entity is IDomainEntityMetadata entityWithMetaData)) continue;

                entityWithMetaData.ChangedAt = now;
                entityWithMetaData.ChangedBy = currentUserName;

                if (entityEntry.Entity is IDomainEntitySoftUpdate) continue;

                entityEntry.Property(nameof(entityWithMetaData.CreatedAt)).IsModified = false;
                entityEntry.Property(nameof(entityWithMetaData.CreatedBy)).IsModified = false;
            }
            
            var deletedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);

            foreach (var entityEntry in deletedEntities)
            {
                if (entityEntry.Entity is not IDomainEntitySoftDelete softDeleteEntity) continue;

                softDeleteEntity.DeletedAt = now;
                softDeleteEntity.DeletedBy = currentUserName;

                entityEntry.State = EntityState.Modified;
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