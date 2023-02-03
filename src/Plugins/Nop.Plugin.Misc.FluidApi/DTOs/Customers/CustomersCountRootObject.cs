using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; } 
    }
}