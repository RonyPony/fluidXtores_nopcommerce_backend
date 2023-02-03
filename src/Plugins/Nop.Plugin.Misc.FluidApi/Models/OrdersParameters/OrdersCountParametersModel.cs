using Nop.Plugin.Misc.FluidApi.ModelBinders;

namespace Nop.Plugin.Misc.FluidApi.Models.OrdersParameters
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

    [ModelBinder(typeof(ParametersModelBinder<OrdersCountParametersModel>))]
    public class OrdersCountParametersModel : BaseOrdersParametersModel
    {
        public OrdersCountParametersModel()
        {
            SinceId = Configurations.DefaultSinceId;
        }

        /// <summary>
        /// Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }
    }
}