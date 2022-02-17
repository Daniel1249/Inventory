using AutoMapper;
using InventoryDatabaseCore;
using InventoryModels.Dtos;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace InventoryDatabaseLayer
{
    public class ItemsRepo : IItemsRepo
    {
        private readonly IMapper _mapper;
        private readonly InventoryDbContext _context;
        public ItemsRepo(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public List<ItemDto> GetItems()
        {
            var items = _context.Items.ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                        .ToList();
            return items;
        }

        public List<ItemDto> GetItemsByDateRange(DateTime minDateValue, DateTime maxDateValue)
        {
            var items = _context.Items.Include(x => x.Category)
                                .Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                                .ToList();
            return items;
        }

        public List<GetItemsForListingDto> GetItemsForListingFromProcedure()
        {
            return _context.ItemsForListing.FromSqlRaw("EXECUTE dbo.GetItemsForListing").ToList();
        }

        public List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive)
        {
            var isActiveParm = new SqlParameter("IsActive", 1);
            return _context.GetItemsTotalValues
            .FromSqlRaw("SELECT * from [dbo].[GetItemsTotalValue] (@IsActive)",
            isActiveParm)
            .ToList();
        }

        public List<FullItemDetailDto> GetItemsWithGenresAndCategories()
        {
            return _context.FullItemDetailDtos
                            .FromSqlRaw("SELECT * FROM [dbo].[vwFullItemDetails]")
                            .AsEnumerable()
                            .OrderBy(x => x.ItemName).ThenBy(x => x.GenreName)
                            .ThenBy(x => x.Category).ThenBy(x => x.PlayerName)
                            .ToList();
        }
    }
}