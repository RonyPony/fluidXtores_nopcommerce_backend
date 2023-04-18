using Newtonsoft.Json;

namespace Nop.Plugin.Misc.ApiFlex.DTO
{
    [JsonObject(Title = "address")]
    public class AddressWithCoordinatesDto : AddressDto
    {
        /// <summary>
        /// Gets or sets the address latitude
        /// </summary>
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the address longitude
        /// </summary>
        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }
}