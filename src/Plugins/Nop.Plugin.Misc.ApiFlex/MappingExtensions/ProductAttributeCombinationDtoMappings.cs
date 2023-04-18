using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Products;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ProductAttributeCombinationDtoMappings
    {
        public static ProductAttributeCombinationDto ToDto(this ProductAttributeCombination productAttributeCombination)
        {
            return productAttributeCombination.MapTo<ProductAttributeCombination, ProductAttributeCombinationDto>();
        }
    }
}