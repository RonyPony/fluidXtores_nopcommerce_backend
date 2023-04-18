using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; } 
    }
}