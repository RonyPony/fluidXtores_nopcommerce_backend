using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.ProductManufacturerMappings;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ProductManufacturerMappingDtoMappings
    {
        public static ProductManufacturerMappingsDto ToDto(this ProductManufacturer mapping)
        {
            return mapping.MapTo<ProductManufacturer, ProductManufacturerMappingsDto>();
        }
    }
}