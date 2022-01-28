using AutoMapper;
using InventoryModels;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore_Activity0302
{
    public class InventoryMapper : Profile
    {
        public InventoryMapper()
        {
            CreateMaps();
        }
        private void CreateMaps()
        {
            CreateMap<Item, ItemDto>();
            // CreateMap<Category, CategoryDto>();

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

            //For IGNORE use:
            //.ForMember(x => x.AFieldNotMappable, opt => opt.Ignore());
        }
    }
}
