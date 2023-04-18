using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}