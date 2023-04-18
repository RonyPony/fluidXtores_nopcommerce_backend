using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.ApiFlex.DTO.Categories;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ApiFlex.Validators
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