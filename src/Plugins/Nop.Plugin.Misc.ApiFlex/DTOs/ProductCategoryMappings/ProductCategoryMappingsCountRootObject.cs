using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.ProductCategoryMappings
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}