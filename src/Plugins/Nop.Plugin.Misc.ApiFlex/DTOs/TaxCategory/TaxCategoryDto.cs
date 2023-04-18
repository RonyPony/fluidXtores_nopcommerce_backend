using Newtonsoft.Json;
using Nop.Plugin.Misc.ApiFlex.DTO.Base;
using Nop.Plugin.Misc.ApiFlex.Validators;

namespace Nop.Plugin.Misc.ApiFlex.DTO.TaxCategory
{
    [JsonObject(Title = "taxcategory")]
    //[Validator(typeof(TaxCategoryDtoValidator))]
    public class TaxCategoryDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the tax rate
        /// </summary>
        public decimal Rate { get; set; }
    }
}
