using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.DTO.Products;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product)
        {
            return product.MapTo<Product, ProductDto>();
        }

        public static ProductAttributeValueDto ToDto(this ProductAttributeValue productAttributeValue)
        {
            return productAttributeValue.MapTo<ProductAttributeValue, ProductAttributeValueDto>();
        }
    }
}