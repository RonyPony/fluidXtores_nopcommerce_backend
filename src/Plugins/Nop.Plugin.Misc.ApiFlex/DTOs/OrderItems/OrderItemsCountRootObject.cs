using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}