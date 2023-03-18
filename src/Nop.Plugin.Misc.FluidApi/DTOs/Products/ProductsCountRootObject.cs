using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.Products
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}