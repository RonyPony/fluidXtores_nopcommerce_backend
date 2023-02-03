using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.Products;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ProductAttributeCombinationDtoMappings
    {
        public static ProductAttributeCombinationDto ToDto(this ProductAttributeCombination productAttributeCombination)
        {
            return productAttributeCombination.MapTo<ProductAttributeCombination, ProductAttributeCombinationDto>();
        }
    }
}