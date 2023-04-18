using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.ApiFlex.DTO.ProductCategoryMappings;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ApiFlex.Validators
{
    public class ProductCategoryMappingDtoValidator : BaseDtoValidator<ProductCategoryMappingDto>
    {

        #region Constructors

        public ProductCategoryMappingDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetCategoryIdRule();
            SetProductIdRule();
        }

        #endregion

        #region Private Methods

        private void SetCategoryIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.CategoryId, "invalid category_id", "category_id");
        }

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(p => p.ProductId, "invalid product_id", "product_id");
        }

        #endregion

    }
}