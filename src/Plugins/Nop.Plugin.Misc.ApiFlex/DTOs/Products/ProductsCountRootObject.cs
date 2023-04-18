using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Products
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}