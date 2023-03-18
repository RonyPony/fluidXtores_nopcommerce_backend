using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.SpecificationAttributes
{
    public class ProductSpecificationAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}