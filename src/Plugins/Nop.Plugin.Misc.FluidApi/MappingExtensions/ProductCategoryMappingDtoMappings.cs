using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.DTO.ProductCategoryMappings;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ProductCategoryMappingDtoMappings
    {
        public static ProductCategoryMappingDto ToDto(this ProductCategory mapping)
        {
            return mapping.MapTo<ProductCategory, ProductCategoryMappingDto>();
        }
    }
}