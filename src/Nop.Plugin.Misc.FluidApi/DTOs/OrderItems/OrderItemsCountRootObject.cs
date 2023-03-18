using Newtonsoft.Json;

namespace Nop.Plugin.Misc.FluidApi.DTO.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}