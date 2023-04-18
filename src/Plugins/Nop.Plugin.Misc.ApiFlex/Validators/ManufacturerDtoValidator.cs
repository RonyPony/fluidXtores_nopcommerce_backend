using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using System.Collections.Generic;
using Nop.Plugin.Misc.ApiFlex.DTO.Manufacturers;

namespace Nop.Plugin.Misc.ApiFlex.Validators
{
    public class ManufacturerDtoValidator : BaseDtoValidator<ManufacturerDto>
    {

        #region Constructors

        public ManufacturerDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(m => m.Name, "invalid name", "name");
        }

        #endregion

    }
}