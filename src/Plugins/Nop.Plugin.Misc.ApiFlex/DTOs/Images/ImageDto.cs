using Newtonsoft.Json;
using Nop.Plugin.Misc.ApiFlex.Attributes;
using Nop.Plugin.Misc.ApiFlex.DTO.Base;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Images
{
    public class ImageDto : BaseDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("attachment")]
        public string Attachment { get; set; }

        [JsonIgnore]
        public byte[] Binary { get; set; }

        [JsonIgnore]
        public string MimeType { get; set; }

        [JsonProperty("seoFilename")]
        public string SeoFilename { get; set; }
    }
}