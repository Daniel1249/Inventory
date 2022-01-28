using EF_Activity001;
using EF_Activity001.DTOs;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Activity1002_Using_TransparentDataEncryption
{
    class Program
    {
        static IConfigurationRoot _configuration;
        static DbContextOptionsBuilder<AdventureWorksContext> _optionsBuilder;
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AdventureWorks"));
        }

        static void Main(string[] args)
        {
            BuildOptions();
            Console.WriteLine("Would you like to view all salespeople? [y/n]");
            var input = Console.ReadLine();
            if (input.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                ShowAllSalesPeople();
            }

            input = string.Empty;
            Console.WriteLine("Would you like to view all salespeople using projections? [y/n]");
            input = Console.ReadLine();
            if (input.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                ShowAllSalesPeopleUsingProjection();
            }

            input = string.Empty;
            Console.WriteLine("Would you like to view the sales report?");
            input = Console.ReadLine();
            if (input.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                //GenerateSalesReport();
                GenerateSalesReportToDTO();
            }
        }

        private static void ShowAllSalesPeople()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                //var salesPeople = db.SalesPerson.Take(20).ToList();
                //foreach (var sp in salesPeople)
                //{
                //    var x = db.Person.FirstOrDefault(x => x.BusinessEntityId == sp.BusinessEntityId);
                //    if (x != null)
                //    {
                //        Console.WriteLine($"{sp} | {x.LastName}, {x.FirstName}");
                //    }
                //}

                var salesPeople = db.SalesPerson
                                    .Include(x => x.BusinessEntity)
                                    .ThenInclude(y => y.BusinessEntity)
                                    .Take(20).ToList();

                foreach (var sp in salesPeople)
                {
                    Console.WriteLine($"{sp} | {sp.BusinessEntity.BusinessEntity.LastName}" +
                                            $", {sp.BusinessEntity.BusinessEntity.FirstName}");
                }
            }
        }

        private static void ShowAllSalesPeopleUsingProjection()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesPeople = db.SalesPerson
                                    //.Include(x => x.BusinessEntity)
                                    //.ThenInclude(y => y.BusinessEntity)
                                    .Select(x => new {
                                        x.BusinessEntityId,
                                        x.BusinessEntity.BusinessEntity.FirstName,
                                        x.BusinessEntity.BusinessEntity.LastName,
                                        x.SalesQuota,
                                        x.SalesYtd,
                                        x.SalesLastYear
                                    }).ToList();

                foreach (var sp in salesPeople)
                {
                    Console.WriteLine($"BID: {sp.BusinessEntityId} | Name: {sp.LastName}" +
                            $", {sp.FirstName} | Quota: {sp.SalesQuota} | " +
                            $"YTD Sales: {sp.SalesYtd} | SalesLastYear {sp.SalesLastYear}");
                }
            }
        }

        private static void GenerateSalesReport()
        {
            var filter = GetFilterFromUser();

            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesReportDetails = db.SalesPerson.Select(sp => new
                {
                    beid = sp.BusinessEntityId,
                    sp.BusinessEntity.BusinessEntity.FirstName,
                    sp.BusinessEntity.BusinessEntity.LastName,
                    sp.SalesYtd,
                    Territories = sp.SalesTerritoryHistory
                                    .Select(y => y.Territory.Name),
                    OrderCount = sp.SalesOrderHeader.Count(),
                    TotalProductsSold = sp.SalesOrderHeader
                                            .SelectMany(y => y.SalesOrderDetail)
                                            .Sum(z => z.OrderQty)
                }).Where(srds => srds.SalesYtd > filter)
                    .OrderBy(srds => srds.LastName)
                        .ThenBy(srds => srds.FirstName)
                            .ThenByDescending(srds => srds.SalesYtd)
                    .Take(20).ToList();

                foreach (var srd in salesReportDetails)
                {
                    Console.WriteLine($"{srd.beid}| {srd.LastName}, {srd.FirstName} |" +
                        $"YTD Sales: {srd.SalesYtd} |" +
                        $"{string.Join(',', srd.Territories)} |" +
                        $"Order Count: {srd.OrderCount} |" +
                        $"Products Sold: {srd.TotalProductsSold}");
                }
            }
        }

        private static void GenerateSalesReportToDTO()
        {
            var filter = GetFilterFromUser();

            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesReportDetails = db.SalesPerson.Select(x => new SalesReportListingDto
                {
                    BusinessEntityId = x.BusinessEntityId,
                    FirstName = x.BusinessEntity.BusinessEntity.FirstName,
                    LastName = x.BusinessEntity.BusinessEntity.LastName,
                    SalesYtd = x.SalesYtd,
                    Territories = x.SalesTerritoryHistory.Select(y => y.Territory.Name),
                    TotalOrders = x.SalesOrderHeader.Count(),
                    TotalProductsSold = x.SalesOrderHeader
                                            .SelectMany(y => y.SalesOrderDetail)
                                            .Sum(z => z.OrderQty)
                }).Where(srds => srds.SalesYtd > filter)
                    .OrderBy(srds => srds.LastName)
                        .ThenBy(srds => srds.FirstName)
                            .ThenByDescending(srds => srds.SalesYtd);

                foreach (var srd in salesReportDetails)
                {
                    Console.WriteLine(srd);
                }
            }
        }

        private static decimal GetFilterFromUser()
        {
            Console.WriteLine("What is the minimum amount of sales?");
            var input = Console.ReadLine();
            decimal filter = 0.0m;

            if (!decimal.TryParse(input, out filter))
            {
                Console.WriteLine("Bad input");
                return 0.0m;
            }
            return filter;
        }
    }
}
