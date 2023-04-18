using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.DTO.Products;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
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