using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.FluidApi.DTO.ProductAttributes;
using Nop.Plugin.Misc.FluidApi.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public class ProductAttributeDtoValidator : BaseDtoValidator<ProductAttributeDto>
    {

        #region Constructors

        public ProductAttributeDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
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