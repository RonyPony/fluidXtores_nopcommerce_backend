using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.FluidApi.DTO.Products;
using Nop.Plugin.Misc.FluidApi.DTO.TaxCategory;
using Nop.Plugin.Misc.FluidApi.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public class TaxCategoryDtoValidator : BaseDtoValidator<TaxCategoryDto>
    {
        #region Constructors

        public TaxCategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.Name, "invalid name", "name");
        }

        #endregion
    }
}
