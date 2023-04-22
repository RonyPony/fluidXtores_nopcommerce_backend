﻿using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.ApiFlex.DTO.ProductAttributes;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ApiFlex.Validators
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