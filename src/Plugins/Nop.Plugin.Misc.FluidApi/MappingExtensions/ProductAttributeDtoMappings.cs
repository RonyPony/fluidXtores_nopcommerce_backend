using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.ProductAttributes;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ProductAttributeDtoMappings
    {
        public static ProductAttributeDto ToDto(this ProductAttribute productAttribute)
        {
            return productAttribute.MapTo<ProductAttribute, ProductAttributeDto>();
        }
    }
}
