using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
       

        private static IConfigurationRoot _configuration;

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

    }
}