
using EFCore_Activity0302;
using EFCore_DBLibrary;
using InventoryHelpers;
using InventoryModels;
//using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_Activity0302
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        private const string _systemUserId = "2fd28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserId = "e2eb8989-a81a-4151-8e86-eb95a7961da2";
        static void Main(string[] args)
        {
            BuildOptions();
            EnsureItems();
            UpdateItems();
            //DeleteAllItems();
            ListInventory();
        }
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }

        static void EnsureItems()
        {
            EnsureItem("Batman Begins1", "You either die the hero or live long enough to see yourself become the villain", "Christian Bale, Katie Holmes");
           
            EnsureItem("Inception2", "You mustn't be afraid to dream a little bigger, darling", "Leonardo DiCaprio, Tom Hardy, Joseph Gordon - Levitt" );
           
            EnsureItem("Remember the Titans3", "Left Side, Strong Side", "Denzel Washington, Will Patton" );
           
            EnsureItem("Star Wars: The Empire Strikes Back4", "He will join us or die, master", "Harrison Ford, Carrie Fisher, Mark Hamill");
           
            EnsureItem("Top Gun5", "I feel the need, the need for speed!", "Tom Cruise, Anthony Edwards, Val Kilmer");
}
        private static void EnsureItem(string name, string description, string notes)
        {
            Random r = new Random();
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //determine if item exists:
                var existingItem = db.Items.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

                if (existingItem == null)
                {
                    //doesn't exist, add it.
                    var item = new Item()
                    {
                        Name = name,
                        CreatedByUserId = _loggedInUserId,
                        IsActive = true,
                        Description = description,
                        LastModifiedUserId = "123",
                        LastModifiedDate = DateTime.Now,
                        Notes = notes,
                        Quantity = r.Next()
                    };
                    db.Items.Add(item);
                    db.SaveChanges();
                }
            }
        }
        private static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.OrderBy(x => x.Name).ToList();

                items.ForEach(x => Console.WriteLine($"New Item: {x.Name}"));
            }
        }
        private static void DeleteAllItems()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.ToList();
                db.Items.RemoveRange(items);
                db.SaveChanges();
            }
        }

        private static void UpdateItems()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.ToList();
                foreach (var item in items)
                {
                    item.CurrentOrFinalPrice = 9.99M;
                }
                db.Items.UpdateRange(items);
                db.SaveChanges();
            }
        }
    }

}