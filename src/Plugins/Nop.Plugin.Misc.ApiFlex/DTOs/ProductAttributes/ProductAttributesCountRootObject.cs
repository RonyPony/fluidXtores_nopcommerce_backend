using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.ProductAttributes
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}