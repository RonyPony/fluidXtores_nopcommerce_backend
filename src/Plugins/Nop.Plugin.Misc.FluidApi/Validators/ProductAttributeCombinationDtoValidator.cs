using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.FluidApi.DTO.Products;
using Nop.Plugin.Misc.FluidApi.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public class ProductAttributeCombinationDtoValidator : BaseDtoValidator<ProductAttributeCombinationDto>
    {

        #region Constructors

        public ProductAttributeCombinationDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetAttributesXmlRule();
            SetProductIdRule();
        }

        #endregion

        #region Private Methods

        private void SetAttributesXmlRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.AttributesXml, "invalid attributes xml", "attributes_xml");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ProductId, "invalid product id", "product_id");
        }

        #endregion

    }
}