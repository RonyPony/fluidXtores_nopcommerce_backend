using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.Categories;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
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