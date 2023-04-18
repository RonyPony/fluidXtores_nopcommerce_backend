using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.ApiFlex.DTO.OrderItems;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ApiFlex.Validators
{
    public class OrderItemDtoValidator : BaseDtoValidator<OrderItemDto>
    {

        #region Constructors

        public OrderItemDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetProductIdRule();
            SetQuantityRule();
        }

        #endregion

        #region Private Methods

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.ProductId, "invalid product_id", "product_id");
        }

        private void SetQuantityRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.Quantity, "invalid quanitty", "quantity");
        }

        #endregion

    }
}