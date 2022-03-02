using InventoryModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBusinessLayer
{
    public interface IItemService
    {
        List<ItemDto> GetItems();
        List<ItemDto> GetItemsByDateRange(DateTime minDateValue, DateTime
        maxDateValue);
        List<GetItemsForListingDto> GetItemsForListingFromProcedure();
        List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive);
        string GetAllItemsPipeDelimitedString();
        List<FullItemDetailDto> GetItemsWithGenresAndCategories();

        int UpsertItem(CreateOrUpdateItemDto item);
        void UpsertItems(List<CreateOrUpdateItemDto> item);
        void DeleteItem(int id);
        void DeleteItems(List<int> itemIds);
    }
}
