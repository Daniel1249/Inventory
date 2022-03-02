using AutoMapper;
using AutoMapper.QueryableExtensions;
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

namespace Activity0903_WorkingWithAutomapper
{
    class Program
    {
        static IConfigurationRoot _configuration;
        static DbContextOptionsBuilder<InventoryDbContext> _optionsBuilder;
        private static MapperConfiguration _mapperConfig;
        private static IMapper _mapper;
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            BuildOptions();
            BuildMapper();
            //ListInventory();

            //work fine
            //ListInventoryWithProjection();


            //GetItemsForListingLinq();



            //GetItemsForListingWithParams();
            //AllActiveItemsPipeDelimitedString();
            //GetItemsTotalValues();
            GetItemsWithGenres();

            // ListCategoriesAndColors();
        }

        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
            _mapperConfig = new MapperConfiguration(cfg => {
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

        static void ListInventory()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //code used before DB encryption
                /*
                var items = db.Items.Take(5).OrderBy(x => x.Name).ToList();
                var result = _mapper.Map<List<Item>, List<ItemDto>>(items);
                result.ForEach(x => Console.WriteLine($"New Item: {x}"));
                */
                var result = db.Items.OrderBy(x => x.Name).Take(20)
                                .Select(x => new ItemDto
                                {
                                    Name = x.Name,
                                    Description = x.Description
                                })
                                .ToList();
                result.ForEach(x => Console.WriteLine($"New Item: {x}"));
            }
        }

        static void ListInventoryWithProjection()
        {
            using (var db = new InventoryDbContext(_optionsBuilder.Options))
            {
                //this code is used without DB encryption
                /*
                var items = db.Items.Take(5)
                                .OrderBy(x => x.Name)
                                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                                .ToList();
                items.ForEach(x => Console.WriteLine($"New Item: {x}"));
                */
               
                    var items = db.Items
                    .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                    .ToList();
                    items.OrderBy(x => x.Name).ToList().ForEach(x => Console.
                    WriteLine($"New Item: {x}"));
                
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
                /*
                var results = db.Items.Include(x => x.Category).ToList().Select(x =>
                                new ItemDto
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
                                })
                                .Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                                .OrderBy(y => y.CategoryName).ThenBy(z => z.Name)
                                .ToList();
                foreach (var itemDto in results)
                {
                    Console.WriteLine(itemDto);
                }
                */
                
                var results = db.Items.Select(x => new GetItemsForListingWithDateDto
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

                //var results2 = db.Categories
                //      .Select(x => x.CategoryColor)
                //      .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider).ToList();  //error

                //var results3 = db.Categories
                //        .Select(x => new CategoryDto
                //        {
                //            Category = x.Name,
                //            CategoryColor = new CategoryColorDto { Color = x.CategoryColor.ColorValue }
                //        });

                //foreach (var c in results3)
                //{
                //    Console.WriteLine($"{c.Category} | {c.CategoryColor.Color}");
                //}
            }
        }
    }
}
