using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.FluidApi.DTO.Categories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public class CategoryDtoValidator : BaseDtoValidator<CategoryDto>
    {

        #region Constructors

        public CategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Name, "invalid name", "name");
        }

        #endregion

    }
}