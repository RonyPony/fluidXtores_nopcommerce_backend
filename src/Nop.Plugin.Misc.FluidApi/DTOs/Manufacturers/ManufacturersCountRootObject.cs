using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.Manufacturers
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}