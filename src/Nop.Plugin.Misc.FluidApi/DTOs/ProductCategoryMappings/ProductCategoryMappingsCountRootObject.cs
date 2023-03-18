using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.ProductCategoryMappings
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}