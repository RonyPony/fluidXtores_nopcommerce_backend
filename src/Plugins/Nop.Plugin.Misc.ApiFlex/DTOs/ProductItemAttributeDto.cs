using Newtonsoft.Json;
using Nop.Plugin.Misc.ApiFlex.DTO.Base;

namespace Nop.Plugin.Misc.ApiFlex.DTO
{
    [JsonObject(Title = "attribute")]
    public class ProductItemAttributeDto : BaseDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
