using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}