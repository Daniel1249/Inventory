using EFCore_DBLibrary;
using InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDataMigrator
{
    
    public class BuildCategories
    {
        private readonly InventoryDbContext _context;
        private const string SEED_USER_ID = "31412031-7859-429c-ab21-c2e3e8d98042";

        public BuildCategories(InventoryDbContext context)
        {
            _context = context;
        }

        public void ExecuteSeed()
        {
            //seed categories here...
            if (_context.Categories.Count() == 0)
            {
                _context.Categories.AddRange(
                new Category()
                {
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Name = "Movies",
                    CreatedByUserId="123",
                    LastModifiedUserId= SEED_USER_ID,
                    CategoryDetail = new CategoryDetail()
                    {
                        ColorValue = "#0000FF",
                        ColorName = "Blue"
                    }
                },
                new Category()
                {
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Name = "Books",
                    CreatedByUserId = "123",
                    LastModifiedUserId = SEED_USER_ID,
                    CategoryDetail = new CategoryDetail()
                    {
                        ColorValue = "#FF0000",
                        ColorName = "Red"
                    }
                },
                new Category()
                {
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Name = "Games",
                    CreatedByUserId = "123",
                    LastModifiedUserId = SEED_USER_ID,
                    CategoryDetail = new CategoryDetail()
                    {
                        ColorValue = "#008000",
                        ColorName = "Green"
                    }
                }
 );
                _context.SaveChanges();
            }
        }
    }
}
