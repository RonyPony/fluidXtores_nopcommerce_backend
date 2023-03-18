using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Categories
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}