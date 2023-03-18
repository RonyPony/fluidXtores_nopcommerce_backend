using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Base
{
    public abstract class BaseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}