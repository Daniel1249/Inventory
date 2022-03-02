using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryBusinessLayer;
using InventoryDatabaseCore;
using InventoryHelpers;
using InventoryModels;
using InventoryModels.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Activity1101_LayeringOurSolution
{
    class Program
    {
        static IConfigurationRoot _configuration;
        static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;

        private static IItemService _itemsService;
        private static ICategoriesService _categoriesService;
        private static List<CategoryDto> _categories;

        private const string _systemUserId = "2fd28110-93d0-427d-9207-d55dbca680fa";
        private const string _loggedInUserId = "e2eb8989-a81a-4151-8e86-eb95a7961da2";

        static void Main(string[] args)
        {
            BuildOptions();
            BuildMapper();
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                _itemsService = new ItemService(db, _mapper);
                _categoriesService = new CategoriesService(db, _mapper);
                //ListInventory();
                //GetItemsForListing();
                //GetAllActiveItemsAsPipeDelimitedString();
                //GetItemsTotalValues();
                //GetFullItemDetails();
                //GetItemsForListingLinq();
                ListCategoriesAndColors();

                //insert logic
                Console.WriteLine("Would you like to create items?");
                var createItems = Console.ReadLine().StartsWith("y", StringComparison.
                OrdinalIgnoreCase);
                if (createItems)
                {
                    Console.WriteLine("Adding new Item(s)");
                    CreateMultipleItems();
                    Console.WriteLine("Items added");
                    var inventory = _itemsService.GetItems();
                    inventory.ForEach(x => Console.WriteLine($"Item: {x}"));
                }

                //update logic
                Console.WriteLine("Would you like to update items?");
                var updateItems = Console.ReadLine().StartsWith("y", StringComparison.
                OrdinalIgnoreCase);
                if (updateItems)
                {
                    Console.WriteLine("Updating Item(s)");
                    UpdateMultipleItems();
                    Console.WriteLine("Items updated");
                    var inventory2 = _itemsService.GetItems();
                    inventory2.ForEach(x => Console.WriteLine($"Item: {x}"));
                }


            }

            static void BuildOptions()
            {
                _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
                _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
                _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
                _mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<InventoryMapper>();
                });
                _mapperConfig.AssertConfigurationIsValid();
                _mapper = _mapperConfig.CreateMapper();
            }

            static void BuildMapper()
            {
                var services = new ServiceCollection();
                services.AddAutoMapper(typeof(InventoryMapper));
                _serviceProvider = services.BuildServiceProvider();
            }


            static void CreateMultipleItems()
            {
                Console.WriteLine("Would you like to create items as a batch?");
                bool batchCreate = Console.ReadLine().StartsWith("y", StringComparison.
                OrdinalIgnoreCase);
                var allItems = new List<CreateOrUpdateItemDto>();
                bool createAnother = true;
                while (createAnother == true)
                {
                    var newItem = new CreateOrUpdateItemDto();
                    Console.WriteLine("Creating a new item.");
                    Console.WriteLine("Please enter the name");
                    newItem.Name = Console.ReadLine();
                    Console.WriteLine("Please enter the description");
                    newItem.Description = Console.ReadLine();
                    Console.WriteLine("Please enter the notes");
                    newItem.Notes = Console.ReadLine();
                    Console.WriteLine("Please enter the Category [B]ooks, [M]ovies,[G]ames");
                    newItem.CategoryId = GetCategoryId(Console.ReadLine().Substring(0,
                    1).ToUpper());
                    if (!batchCreate)
                    {
                        _itemsService.UpsertItem(newItem);
                    }
                    else
                    {
                        allItems.Add(newItem);
                    }
                    Console.WriteLine("Would you like to create another item?");
                    createAnother = Console.ReadLine().StartsWith("y",
                    StringComparison.OrdinalIgnoreCase);

                    if (batchCreate && !createAnother)
                    {
                        _itemsService.UpsertItems(allItems);
                    }
                }
            };

            static int GetCategoryId(string input)
            {
                switch (input)
                {
                    case "B":
                        return _categories.FirstOrDefault(x => x.Category.ToLower().
                        Equals("books"))?.Id ?? -1;
                    case "M":
                        return _categories.FirstOrDefault(x => x.Category.ToLower().
                        Equals("movies"))?.Id ?? -1;
                    case "G":
                        return _categories.FirstOrDefault(x => x.Category.ToLower().
                        Equals("games"))?.Id ?? -1;

                        static void ListInventory()
                        {
                            var result = _itemsService.GetItems();
                            result.ForEach(x => Console.WriteLine($"New Item: {x}"));
                        }
                    default:
                        return -1;
                }
            }

            static void UpdateMultipleItems()
            {
                Console.WriteLine("Would you like to update items as a batch?");
                bool batchUpdate = Console.ReadLine().StartsWith("y", StringComparison.
                OrdinalIgnoreCase);
                var allItems = new List<CreateOrUpdateItemDto>();
                bool updateAnother = true;
                while (updateAnother == true)
                {
                    Console.WriteLine("Items");
                    Console.WriteLine("Enter the ID number to update");
                    Console.WriteLine("*******************************");
                    var items = _itemsService.GetItems();
                    items.ForEach(x => Console.WriteLine($"ID: {x.Id} | {x.Name}"));
                    Console.WriteLine("*******************************");
                    int id = 0;
                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        var itemMatch = items.FirstOrDefault(x => x.Id == id);
                        if (itemMatch != null)
                        {
                            var updItem = _mapper.Map<CreateOrUpdateItemDto>(_mapper.
                            Map<Item>(itemMatch));
                            Console.WriteLine("Enter the new name [leave blank to keep existing]");
                            var newName = Console.ReadLine();
                            updItem.Name = !string.IsNullOrWhiteSpace(newName) ?
                            newName : updItem.Name;
                            Console.WriteLine("Enter the new desc [leave blank to keep existing]");

                            var newDesc = Console.ReadLine();
                            updItem.Description = !string.IsNullOrWhiteSpace(newDesc) ?
                            newDesc : updItem.Description;
                            Console.WriteLine("Enter the new notes [leave blank to keep existing]");
                            var newNotes = Console.ReadLine();
                            updItem.Notes = !string.IsNullOrWhiteSpace(newNotes) ?
                            newNotes : updItem.Notes;
                            Console.WriteLine("Toggle Item Active Status? [y/n]");
                            var toggleActive = Console.ReadLine().Substring(0,
                            1).Equals("y", StringComparison.OrdinalIgnoreCase);
                            if (toggleActive)
                            {
                                updItem.IsActive = !updItem.IsActive;
                            }
                            Console.WriteLine("Enter the category - [B]ooks, [M]ovies, [G]ames, or[N]o Change");
                            var userChoice = Console.ReadLine().Substring(0, 1).ToUpper();
                            updItem.CategoryId = userChoice.Equals("N",
                            StringComparison.OrdinalIgnoreCase) ? itemMatch.CategoryId
                            : GetCategoryId(userChoice);
                            if (!batchUpdate)
                            {
                                _itemsService.UpsertItem(updItem);
                            }
                            else
                            {
                                allItems.Add(updItem);
                            }
                        }
                    }
                }
            }

            static void GetItemsForListing()
            {
                /*
               var minDateValue = new DateTime(2021, 1, 1);
               var maxDateValue = new DateTime(2024, 1, 1);
               using (var db = new InventoryDbContext(_optionsBuilder.Options))
               {
                   var results = db.Items.Select(x => new
                   {
                       x.CreatedDate,
                       CategoryName = x.Category.Name,
                       x.Description,
                       x.IsActive,
                       x.IsDeleted,
                       x.Name,
                       x.Notes
                   }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <=
                   maxDateValue)
                   .OrderBy(y => y.CategoryName).ThenBy(z => z.Name)
                   .ToList();
                   foreach (var item in results)
                   {
                       Console.WriteLine($"ITEM {item.CategoryName}| {item.Name} - {item.Description}");
                   }
               }
               

                var results = _itemsService.GetItemsForListingFromProcedure();
               foreach (var item in results)
               {
                   var output = $"ITEM {item.Name}] {item.Description}";
                   if (!string.IsNullOrWhiteSpace(item.CategoryName))
                   {
                       output = $"{output} has category: {item.Name}";
                   }
                   Console.WriteLine(output);
               }
                
                using (var db = new InventoryDbContext(_optionsBuilder.Options))
                {
                    var results = db.Items.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
                    foreach (var item in results)
                    {
                        Console.WriteLine($"ITEM {item.Category}] {item.Name}");
                    }
                }
                */

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

            static void GetAllActiveItemsAsPipeDelimitedString()
            {
                Console.WriteLine($"All active Items: {_itemsService.GetAllItemsPipeDelimitedString()}");
            }

            static void GetItemsTotalValues()
            {
                var results = _itemsService.GetItemsTotalValues(true);
                foreach (var item in results)
                {
                    Console.WriteLine($"New Item] {item.Id,-10}" +
                    $"|{item.Name,-50}" +
                    $"|{item.Quantity,-4}" +
                    $"|{item.TotalValue,-5}");
                }
            }

            static void GetFullItemDetails()
            {
                var result = _itemsService.GetItemsWithGenresAndCategories();
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

            static void GetItemsForListingLinq()
            {
                var minDateValue = new DateTime(2021, 1, 1);
                var maxDateValue = new DateTime(2024, 1, 1);
                var results = _itemsService.GetItemsByDateRange(minDateValue, maxDateValue)
                .OrderBy(y => y.Name).ThenBy(z => z.Name);
                foreach (var itemDto in results)
                {
                    Console.WriteLine(itemDto);
                }
            }

            static void ListCategoriesAndColors()
            {
                /*
                        var results = _categoriesService.ListCategoriesAndDetails();
                        _categories = results;

                        foreach (var c in results)
                        {
                            Console.WriteLine($"Category [{c.Category}] is {c.CategoryDetail.Color}");
                        }
                */
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
        }
    }
}

                
            
        
    
    

        /*

            private static void GetFullItemDetails()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
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
        static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var items = db.Items.Take(5).OrderBy(x => x.Name).ToList();
                var result = _mapper.Map<List<Item>, List<ItemDto>>(items);
                result.ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }

        static void ListInventoryWithProjection()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //var items = db.Items.Take(5)
                //                .OrderBy(x => x.Name)
                //                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                //                .ToList();
                var items = db.Items.OrderBy(x => x.Name).Take(5)
                                    .Select(x => new ItemDto
                                    {
                                        Name = x.Name,
                                        Description = x.Description
                                    })
                                    .ToList();

                items.ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }

        static void ListInventoryWithAlwaysEncrypted()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var theItems = db.Items.ToList().OrderBy(x => x.Name).Take(5);
                var items = _mapper.Map<List<ItemDto>>(theItems);
                items.ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }

        static void GetItemsForListingWithParams()
        {
            var minDate = new SqlParameter("minDate", new DateTime(2020, 1, 1));
            var maxDate = new SqlParameter("maxDate", new DateTime(2021, 1, 1));

            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.ItemsForListing
                                .FromSqlRaw("EXECUTE dbo.GetItemsForListing @minDate, @maxDate", minDate, maxDate)
                                .ToList();
                foreach (var item in results)
                {
                    Console.WriteLine($"ITEM {item.Name} - {item.Description}");
                }
            }
        }

        static void AllActiveItemsPipeDelimitedString()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);

                var result = db.AllItemsOutput
                                .FromSqlRaw("SELECT [dbo].[ItemNamesPipeDelimitedString] (@IsActive) AllItems", isActiveParm)
                                .FirstOrDefault();

                Console.WriteLine($"All active Items: {result.AllItems}");
            }
        }

        static void GetItemsTotalValues()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var isActiveParm = new SqlParameter("IsActive", 1);

                var result = db.GetItemsTotalValues
                                .FromSqlRaw("SELECT * from [dbo].[GetItemsTotalValue] (@IsActive)", isActiveParm)
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

        static void GetItemsWithGenres()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var result = db.ItemsWithGenres.ToList();

                foreach (var item in result)
                {
                    Console.WriteLine($"New Item] {item.Id,-10}" +
                                        $"|{item.Name,-50}" +
                                        $"|{item.Genre ?? "",-4}");
                }
            }
        }

        static void GetItemsForListingLinq()
        {
            var minDateValue = new DateTime(2020, 1, 1);
            var maxDateValue = new DateTime(2021, 1, 1);

            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Items.Include(x => x.Category).ToList()
                                .Select(x => new GetItemsForListingWithDateDto
                                {
                                    CreatedDate = x.CreatedDate,
                                    CategoryName = x.Category.Name,
                                    Description = x.Description,
                                    IsActive = x.IsActive,
                                    IsDeleted = x.IsDeleted,
                                    Name = x.Name,
                                    Notes = x.Notes
                                }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                    .OrderBy(y => y.CategoryName).ThenBy(z => z.Name).ToList();

                foreach (var item in results)
                {
                    Console.WriteLine($"ITEM {item.CategoryName}| {item.Name} - {item.Description}");
                }
            }
        }

        static void ListCategoriesAndColors()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                var results = db.Categories
                        .Include(x => x.CategoryColor)
                        .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider).ToList();

                foreach (var c in results)
                {
                    Console.WriteLine($"{c.Category} | {c.CategoryColor.Color}");
                }
            }
        }
    }
}
        */
