using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.DTO.ProductManufacturerMappings
{
    public class ProductManufacturerMappingsRootObject : ISerializableObject
    {
        public ProductManufacturerMappingsRootObject()
        {
            ProductManufacturerMappingsDtos = new List<ProductManufacturerMappingsDto>();
        }

        [JsonProperty("product_manufacturer_mappings")]
        public IList<ProductManufacturerMappingsDto> ProductManufacturerMappingsDtos { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_manufacturer_mappings";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ProductManufacturerMappingsDto);
        }
    }
}