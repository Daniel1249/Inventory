using EFCore_DBLibrary;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
namespace InventoryDataMigrator
{
    class Program
    {
        static IConfigurationRoot _configuration;

        static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.
           ConfigurationRoot;

            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();

            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString
           ("InventoryManager"));
        }
        static void Main(string[] args)
        {
            BuildOptions();
            ApplyMigrations();

            ExecuteCustomSeedData();


        }

        private static void ApplyMigrations()

        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                db.Database.Migrate();
            }
        }

        private static void ExecuteCustomSeedData()
        {
            using (var context = new InventoryDbContext(_optionsBuilder.Options))
            {
                var categories = new BuildCategories(context);
                categories.ExecuteSeed();

                //to seed also the Items table  - before seed Db, make sur the BB is clean: DELETE FROM ItemPlayers; DELETE FROM Items;
                var items = new BuildItems(context);
                items.ExecuteSeed();
            }
        }
    }
}