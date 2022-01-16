using EF_Activity001;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Activity0801_Sorting_Filtering_Paging
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<AdventureWorksContext> _optionsBuilder;

        static void Main(string[] args)
        {
            BuildOptions();
            //ListPeopleThenOrderAndTake();
            //QueryPeopleOrderedToListAndTake();

            Console.WriteLine("Please Enter the partial First or Last Name, or the Person Type to search for:");
                    var result = Console.ReadLine();
                    //FilteredPeople(result);

            int pageSize = 7;
            for (int pageNumber = 0; pageNumber < 10; pageNumber++)
            {
                Console.WriteLine($"Page {pageNumber + 1}");
                FilteredAndPagedResult(result, pageNumber, pageSize);
            }
        }

        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AdventureWorks"));
        }

        private static void FilteredAndPagedResult(string filter, int pageNumber, int pageSize)
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var searchTerm = filter.ToLower();
                var query = db.Person.Where(x => x.LastName.ToLower().
                Contains(searchTerm)
                || x.FirstName.ToLower().
                Contains(searchTerm)
                || x.PersonType.ToLower().
                Equals(searchTerm))
                .OrderBy(x => x.LastName)
                .Skip(pageNumber * pageSize)
                .Take(pageSize);
                foreach (var person in query)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}, { person.PersonType}");
                }
            }
        }
        private static void FilteredPeople(string filter)
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var searchTerm = filter.ToLower();
                var query = db.Person.Where(x => x.LastName.ToLower().
                Contains(searchTerm)
                || x.FirstName.ToLower().
                Contains(searchTerm)
                || x.PersonType.ToLower().
                Equals(searchTerm));
                foreach (var person in query)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName},{ person.PersonType}");
                }
            }
        }

        private static void ListPeopleThenOrderAndTake()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var people = db.Person.ToList().OrderByDescending(x => x.LastName);
                foreach (var person in people.Take(10))
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}");
                }
            }
        }
        private static void QueryPeopleOrderedToListAndTake()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var query = db.Person.OrderByDescending(x => x.LastName);
                var result = query.Take(10);
                foreach (var person in result)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}");
                }
            }
        }
    }
}
