using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.SpecificationAttributes;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.SpecificationAttributes;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Linq;
using System.Net;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    public class SpecificationAttributesController : BaseApiController
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly IDTOHelper _dtoHelper;

        public SpecificationAttributesController(IJsonFieldsSerializer jsonFieldsSerializer, 
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  ISpecificationAttributeService specificationAttributeService,
                                  ISpecificationAttributeApiService specificationAttributesApiService,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _specificationAttributeService = specificationAttributeService;
            _specificationAttributeApiService = specificationAttributesApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Receive a list of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetSpecificationAttributesAsync(SpecifcationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var specificationAttribtues = _specificationAttributeApiService.GetSpecificationAttributesAsync(limit: parameters.Limit, page: parameters.Page, sinceId: parameters.SinceId);

            var specificationAttributeDtos = specificationAttribtues.Select(x => _dtoHelper.PrepareSpecificationAttributeDto(x)).ToList();

            var specificationAttributesRootObject = new SpecificationAttributesRootObjectDto()
            {
                SpecificationAttributes = specificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/count")]
        [ProducesResponseType(typeof(SpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]        
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetSpecificationAttributesCountAsync(SpecifcationAttributesCountParametersModel parameters)
        {
            var specificationAttributesCount = (await _specificationAttributeService.GetSpecificationAttributesAsync()).Count();

            var specificationAttributesCountRootObject = new SpecificationAttributesCountRootObject()
            {
                Count = specificationAttributesCount
            };

            return Ok(specificationAttributesCountRootObject);
        }

        /// <summary>
        /// Retrieve specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the specification  attribute</param>
        /// <param name="fields">Fields from the specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetSpecificationAttributeByIdAsync(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);

            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificationAttributesRootObject = new SpecificationAttributesRootObjectDto();
            specificationAttributesRootObject.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/specificationattributes")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateSpecificationAttributeAsync([ModelBinder(typeof(JsonModelBinder<SpecificationAttributeDto>))] Delta<SpecificationAttributeDto> specificaitonAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var specificationAttribute = new SpecificationAttribute();
            specificaitonAttributeDelta.Merge(specificationAttribute);

            _specificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);

            await CustomerActivityService.InsertActivityAsync("AddNewSpecAttribute", await LocalizationService.GetResourceAsync("ActivityLog.AddNewSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificationAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificationAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateSpecificationAttributeAsync([ModelBinder(typeof(JsonModelBinder<SpecificationAttributeDto>))] Delta<SpecificationAttributeDto> specificationAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            int specificationAttributeId = specificationAttributeDelta.Dto.Id;

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            specificationAttributeDelta.Merge(specificationAttribute);

            await _specificationAttributeService.UpdateSpecificationAttributeAsync(specificationAttribute);
          
            await CustomerActivityService.InsertActivityAsync("EditSpecAttribute",await LocalizationService.GetResourceAsync("ActivityLog.EditSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product attribute
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificatoinAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificatoinAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> DeleteSpecificationAttributeAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteSpecAttribute",await LocalizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute);

         //InsertActivityRawJsonActionResult("{}");
        }
    }
}