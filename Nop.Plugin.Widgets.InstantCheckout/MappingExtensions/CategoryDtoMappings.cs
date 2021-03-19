using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.InstantCheckout.AutoMapper;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Categories;

namespace Nop.Plugin.Widgets.InstantCheckout.MappingExtensions
{
    public static class CategoryDtoMappings
    {
        public static CategoryDto ToDto(this Category category)
        {
            return category.MapTo<Category, CategoryDto>();
        }

        public static Category ToEntity(this CategoryDto categoryDto)
        {
            return categoryDto.MapTo<CategoryDto, Category>();
        }
    }
}