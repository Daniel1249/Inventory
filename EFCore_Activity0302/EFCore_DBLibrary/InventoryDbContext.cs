using InventoryModels;
using InventoryModels.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {


        private static IConfigurationRoot _configuration;
        private const string _systemUserId = "2fd28110-93d0-427d-9207-d55dbca680fa";

        //Add a default constructor if scaffolding is needed
        public InventoryDbContext()
        {
        }

        //Add a complex constructor for allowing  Dependency Injection
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
           : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDetail> CategoryDetails { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GetItemsForListingDto> ItemsForListing { get; set; }

        public DbSet<AllItemsPipeDelimitedStringDto> AllItemsOutput { get; set; }

        public DbSet<GetItemsTotalValueDto> GetItemsTotalValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true,
               reloadOnChange: true);
                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("InventoryManager");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
            .HasMany(x => x.Players)
            .WithMany(p => p.Items)
            .UsingEntity<Dictionary<string, object>>(
            "ItemPlayers",
            ip => ip.HasOne<Player>()
            .WithMany()
            .HasForeignKey("PlayerId")
            .HasConstraintName("FK_ItemPlayer_Players_PlayerId")
            .OnDelete(DeleteBehavior.Cascade),
            ip => ip.HasOne<Item>()
            .WithMany()
            .HasForeignKey("ItemId")
            .HasConstraintName("FK_PlayerItem_Items_ItemId")
            .OnDelete(DeleteBehavior.ClientCascade)
            );

            modelBuilder.Entity<GetItemsForListingDto>(x =>
            {
                x.HasNoKey();
                x.ToView("ItemsForListing");
            });

            modelBuilder.Entity<AllItemsPipeDelimitedStringDto>(x => {
                x.HasNoKey();
                x.ToView("AllItemsOutput");
            });

            modelBuilder.Entity<GetItemsTotalValueDto>(x =>
            {
                x.HasNoKey();
                x.ToView("GetItemsTotalValues");
            });

            //SEED DATA FOR GENRE tabel

            var genreCreateDate = new DateTime(2021, 01, 01);
            modelBuilder.Entity<Genre>(x =>
            {
                x.HasData(
                new Genre()
                {
                    Id = 1,
                    CreatedDate = genreCreateDate,
                    IsActive =
               true,
                    IsDeleted = false,
                    Name = "Fantasy"
                },
                new Genre()
                {
                    Id = 2,
                    CreatedDate = genreCreateDate,
                    IsActive =
               true,
                    IsDeleted = false,
                    Name = "Sci/Fi"
                },
                new Genre()
                {
                    Id = 3,
                    CreatedDate = genreCreateDate,
                    IsActive =
               true,
                    IsDeleted = false,
                    Name = "Horror"
                },
                new Genre()
                {
                    Id = 4,
                    CreatedDate = genreCreateDate,
                    IsActive =
               true,
                    IsDeleted = false,
                    Name = "Comedy"
                },
                new Genre()
                {
                    Id = 5,
                    CreatedDate = genreCreateDate,
                    IsActive =
               true,
                    IsDeleted = false,
                    Name = "Drama"
                }
                );
            });
            
        }

        /*
        public override int SaveChanges()
        {
            var tracker = ChangeTracker;
            foreach (var entry in tracker.Entries())
            {
                System.Diagnostics.Debug.WriteLine($"{entry.Entity} has state {entry.State}");
            }
            return base.SaveChanges();
        }
        */
        public override int SaveChanges()
        {
            var tracker = ChangeTracker;
            foreach (var entry in tracker.Entries())
            {
                if (entry.Entity is FullAuditModel)
                {
                    var referenceEntity = entry.Entity as FullAuditModel;
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            referenceEntity.CreatedDate = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(referenceEntity.CreatedByUserId))
                                {
                                    referenceEntity.CreatedByUserId = _systemUserId;
                                }
                            break;

                        case EntityState.Deleted:

                        case EntityState.Modified:
                            referenceEntity.LastModifiedDate = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(referenceEntity.
                           LastModifiedUserId))
                            {
                                referenceEntity.LastModifiedUserId = _systemUserId;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}