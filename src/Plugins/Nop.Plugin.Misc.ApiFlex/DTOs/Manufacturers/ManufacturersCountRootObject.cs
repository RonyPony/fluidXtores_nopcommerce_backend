using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Manufacturers
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}