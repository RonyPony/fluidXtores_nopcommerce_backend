using Newtonsoft.Json;
using Nop.Plugin.Misc.FluidApi.DTO.Base;
using Nop.Plugin.Misc.FluidApi.Validators;

namespace Nop.Plugin.Misc.FluidApi.DTO.ProductManufacturerMappings
{
    [JsonObject(Title = "product_manufacturer_mapping")]
    //[Validator(typeof(ProductManufacturerMappingDtoValidator))]
    public class ProductManufacturerMappingsDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        [JsonProperty("manufacturer_id")]
        public int? ManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        [JsonProperty("is_featured_product")]
        public bool? IsFeaturedProduct { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }
    }
}