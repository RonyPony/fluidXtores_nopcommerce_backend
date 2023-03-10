using Newtonsoft.Json;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;
using Nop.Plugin.Misc.FluidApi.ModelBinders;

namespace Nop.Plugin.Misc.FluidApi.Models.OrderItemsParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<OrderItemsParametersModel>))]
    public class OrderItemsParametersModel
    {
        public OrderItemsParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            Fields = string.Empty;
        }
        
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}