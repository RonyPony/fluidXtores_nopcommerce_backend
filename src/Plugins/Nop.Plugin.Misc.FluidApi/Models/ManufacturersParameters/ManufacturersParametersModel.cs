using System.Collections.Generic;
using Newtonsoft.Json;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Misc.FluidApi.Models.ManufacturersParameters
{
    // JsonProperty is used only for swagger
    [ModelBinder(typeof(ParametersModelBinder<ManufacturersParametersModel>))]
    public class ManufacturersParametersModel : BaseManufacturersParametersModel
    {
        public ManufacturersParametersModel()
        {
            Ids = null;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = Configurations.DefaultSinceId;
            Fields = string.Empty;
        }

        /// <summary>
        /// A comma-separated list of manufacturer ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        /// <summary>
        /// Amount of results (default: 50) (maximum: 250)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Page to show (default: 1)
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Restrict results to after the specified ID
        /// </summary>
        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        /// <summary>
        /// comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("fields")]
        public string Fields { get; set; }

        /// <summary>
        /// comma-separated list of fields to include in the response
        /// </summary>
        [JsonProperty("languageid")]
        public int? LanguageId { get; set; }
    }
}