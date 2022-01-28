
using EFCore_Activity0302;
using EFCore_DBLibrary;
using InventoryHelpers;
using InventoryModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using InventoryModels.DTOs;
using AutoMapper.QueryableExtensions;

namespace EFCore_Activity0302
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        private const string _systemUserId = "2fd28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserId = "e2eb8989-a81a-4151-8e86-eb95a7961da2";

        //for AutoMapper config
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            BuildOptions();
            BuildMapper();

            //GetItemsForListingLinq();
            //EnsureCategory();
            //EnsureItems();
            //UpdateItems();
            //DeleteAllItems();

            //ListInventory();

            //GetItemsForListing();

            //GetItemsTotalValues();

            //GetAllActiveItemsAsPipeDelimitedString();

            //GetFullItemDetails();

            //ListInventoryWithProjection();

            ListCategoriesAndColors();
        }



        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
        }

        static void BuildMapper()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(InventoryMapper));
            _serviceProvider = services.BuildServiceProvider();

            //set up the configuration and tell AutoMapper to use the InventoryMapper profile
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InventoryMapper>();
            });
            _mapperConfig.AssertConfigurationIsValid();
            _mapper = _mapperConfig.CreateMapper();

        }

        private static void ListCategoriesAndColors()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Categories
                .Include(x => x.CategoryDetail)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider).
                ToList();

                foreach (var c in results)
                {
                    Console.WriteLine($"Category [{c.Category}] is {c.CategoryDetail.Color}");
                }
            }
        }

        private static void ListInventoryWithProjection()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items
                .OrderBy(x => x.Name)
                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                .ToList();
                items.ForEach(x => Console.WriteLine($"New Item: {x}"));
            }

        }
        static void EnsureItems()
        {
            EnsureItem("Batman Begins1", "You either die the hero or live long enough to see yourself become the villain", "Christian Bale, Katie Holmes");

            EnsureItem("Inception2", "You mustn't be afraid to dream a little bigger, darling", "Leonardo DiCaprio, Tom Hardy, Joseph Gordon - Levitt");

            EnsureItem("Remember the Titans3", "Left Side, Strong Side", "Denzel Washington, Will Patton");

            EnsureItem("Star Wars: The Empire Strikes Back4", "He will join us or die, master", "Harrison Ford, Carrie Fisher, Mark Hamill");

            EnsureItem("Top Gun5", "I feel the need, the need for speed!", "Tom Cruise, Anthony Edwards, Val Kilmer");
        }

        static void EnsureCategory()
        {
            EnsureCategory("Movies");
        }

        private static void EnsureCategory(string name)
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //determine if category exists:
                var existingCategory = db.Categories.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

                if (existingCategory == null)
                {
                    //doesn't esist, add it
                    db.Categories.AddRange(
                    new Category()
                    {
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        LastModifiedUserId = _loggedInUserId,

                        IsDeleted = false,
                        Name = name,
                        CategoryDetail = new CategoryDetail()
                        {
                            ColorValue = "#0000FF",
                            ColorName = "Blue"
                        }
                    });

                    db.SaveChanges();
                }
            }
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
                        Quantity = r.Next(1, 1000),

                        CategoryId = 9
                    };
                    db.Items.Add(item);
                    db.SaveChanges();
                }
            }
        }

        //using direct map into Dto
        private static void GetItemsForListingLinq()
        {
            var minDateValue = new DateTime(2021, 1, 1);
            var maxDateValue = new DateTime(2024, 1, 1);
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.Select(x => new ItemDto
                {
                    CreatedDate = x.CreatedDate,
                    CategoryName = x.Category.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Name = x.Name,
                    Notes = x.Notes,
                    CategoryId = x.Category.Id,
                    Id = x.Id
                }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                    .OrderBy(y => y.CategoryName).ThenBy(z => z.Name)
                    .ToList();

                foreach (var itemDto in results)
                {
                    Console.WriteLine(itemDto);
                }

                /*
                foreach (var item in results)
                {
                    Console.WriteLine($"ITEM {item.CategoryName}| {item.Name} - {item.Description}");
                }
                */
            }
        }
        private static void GetFullItemDetails()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.DetailedItems.ToList();


                var result = db.FullItemDetailDtos
                .FromSqlRaw("SELECT * FROM [dbo].[vwFullItemDetails] " + "ORDER BY ItemName, GenreName,Category, PlayerName ")
                .ToList();
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id,-10}" +
                    $"|{item.ItemName,-50}" +
                    $"|{item.ItemDescription,-4}" +
                    $"|{item.PlayerName,-5}" +
                    $"|{item.Category,-5}" +
                    $"|{item.GenreName,-5}");
                }

            }
        }

        private static void GetAllActiveItemsAsPipeDelimitedString()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.AllItemsOutput
                                .FromSqlRaw("SELECT [dbo].[ItemNamesPipeDelimitedString](@IsActive)AllItems", isActiveParm).FirstOrDefault();

                Console.WriteLine($"All active Items: {result.AllItems}");
            }
        }

        //this method is using AutoMapper
        private static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                // var items = db.Items.OrderBy(x => x.Name).ToList();

                var items = db.Items.OrderBy(x => x.Name).ToList();
                var result = _mapper.Map<List<Item>, List<ItemDto>>(items);



                items.ForEach(x => Console.WriteLine($"New Item: {x.Name}" + $"Item Description: {x.Description}"));
            }
        }


        private static void GetItemsForListing2()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
                foreach (var item in results)
                {
                    Console.WriteLine($"ITEM {item.Id}] {item.Name}");
                }
            }
        }


        private static void GetItemsForListing()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {

                var results = db.ItemsForListing.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
                foreach (var item in results)
                {
                    var output = $"ITEM {item.Name}] {item.Description}";
                    if (!string.IsNullOrWhiteSpace(item.CategoryName))
                    {
                        output = $"{output} has category: {item.CategoryName}";
                    }
                    Console.WriteLine(output);
                }
            }
        }

        private static void GetItemsTotalValues()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);
                var result = db.GetItemsTotalValues
                .FromSqlRaw("SELECT * from [dbo].[GetItemsTotalValue](@IsActive)", isActiveParm)
                .ToList();
                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id,-10}" +
                    $"|{item.Name,-50}" +
                    $"|{item.Quantity,-4}" +
                    $"|{item.TotalValue,-5}");
                }
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
                    item.CurrentOrFinalPrice = 8.88M;
                    item.CategoryId = 9;
                }
                db.Items.UpdateRange(items);
                db.SaveChanges();
            }
        }
    }

}