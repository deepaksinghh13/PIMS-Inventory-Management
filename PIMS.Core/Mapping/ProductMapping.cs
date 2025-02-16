using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PIMS.Core.Dto.CategoriesDto;
using PIMS.Core.Dto.InventoryDto;
using PIMS.Core.Dto.ProductDto;
using PIMS.Core.Models;

namespace PIMS.Core.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());

            CreateMap<Product, ProductsDto>();

            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore());

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<CreateInventoryDto, Inventory>().ReverseMap();
            CreateMap<Inventory, InventoriesDto>().ReverseMap();
        }
    }
}
