using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}