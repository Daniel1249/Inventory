using AutoMapper;
using InventoryModels;
using InventoryModels.Dtos;

namespace Activity1101_LayeringOurSolution
{
    public class InventoryMapper : Profile
    {
        public InventoryMapper()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {
            CreateMap<Item, ItemDto>().ReverseMap();

            CreateMap<Category, CategoryDto>()
                 .ForMember(x => x.Category, opt => opt.MapFrom(y => y.Name))
                 .ReverseMap()
                 .ForMember(y => y.Name, opt => opt.MapFrom(x => x.Category));

            CreateMap<CategoryDetail, CategoryDetailDto>()
                .ForMember(x => x.Color, opt => opt.MapFrom(y => y.ColorName))
                .ForMember(x => x.Value, opt => opt.MapFrom(y => y.ColorValue))
                .ReverseMap()
                .ForMember(y => y.ColorValue, opt => opt.MapFrom(x => x.Value))
                .ForMember(y => y.ColorName, opt => opt.MapFrom(x => x.Color));

            CreateMap<CategoryColor, CategoryColorDto>()
                 .ForMember(x => x.Color, opt => opt.MapFrom(y => y.ColorValue))
                 .ReverseMap()
                 .ForMember(y => y.ColorValue, opt => opt.MapFrom(x => x.Color));
            CreateMap<Item, CreateOrUpdateItemDto>()
                .ReverseMap()
                .ForMember(x => x.Category, opt => opt.Ignore());

        }
    }
}
