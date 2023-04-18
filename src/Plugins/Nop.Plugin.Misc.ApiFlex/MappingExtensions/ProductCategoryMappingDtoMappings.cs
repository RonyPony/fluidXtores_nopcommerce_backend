using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.DTO.ProductCategoryMappings;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ProductCategoryMappingDtoMappings
    {
        public static ProductCategoryMappingDto ToDto(this ProductCategory mapping)
        {
            return mapping.MapTo<ProductCategory, ProductCategoryMappingDto>();
        }
    }
}