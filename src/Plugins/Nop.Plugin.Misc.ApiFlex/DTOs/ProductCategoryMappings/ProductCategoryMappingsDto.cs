using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.DTO.Base;

namespace Nop.Plugin.Misc.ApiFlex.DTO.ProductCategoryMappings
{
    [JsonObject(Title = "product_category_mapping")]
    //[Validator(typeof(ProductCategoryMappingDtoValidator))]
    public class ProductCategoryMappingDto : BaseDto
    {
        public void Merge(ref ProductCategory productCategoryMapping)
        {
            productCategoryMapping.ProductId = (int)this.ProductId;
            productCategoryMapping.CategoryId = (int)this.CategoryId;
            productCategoryMapping.IsFeaturedProduct = true;
            productCategoryMapping.DisplayOrder = 99;
        }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }

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