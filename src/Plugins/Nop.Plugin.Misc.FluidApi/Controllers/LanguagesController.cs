using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.Languages;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using NUglify.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{

    public class LanguagesController : BaseApiController
    {
        private ILanguageService _languageService;
        private readonly IDTOHelper _dtoHelper;

        public LanguagesController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ILanguageService languageService,
            IDTOHelper dtoHelper)
            : base(jsonFieldsSerializer,
                  aclService,
                  customerService,
                  storeMappingService,
                  storeService,
                  discountService,
                  customerActivityService,
                  localizationService,
                  pictureService)
        {
            _languageService = languageService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Retrieve all languages
        /// </summary>
        /// <param name="fields">Fields from the language you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/languages")]
        [ProducesResponseType(typeof(LanguagesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetAllLanguagesAsync(string fields = "")
        {
            var allLanguages = _languageService.GetAllLanguages();

            IList<LanguageDto> languagesAsDto = new List<LanguageDto>();
            foreach (var item in allLanguages)
            {
                languagesAsDto.Add(await _dtoHelper.PrepareLanguageDtoAsync(item));
            }

            var languagesRootObject = new LanguagesRootObject()
            {
                Languages = languagesAsDto
            };

            var json = JsonFieldsSerializer.Serialize(languagesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
