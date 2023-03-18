using Newtonsoft.Json;
using Nop.Plugin.Misc.FluidApi.DTO.Base;

namespace Nop.Plugin.Misc.FluidApi.DTO
{
    [JsonObject(Title = "attribute")]
    public class ProductItemAttributeDto : BaseDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
