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
using Nop.Plugin.Misc.FluidApi.Models.ProductSpecificationAttributes;
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
    public class ProductSpecificationAttributesController : BaseApiController
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly IDTOHelper _dtoHelper;

        public ProductSpecificationAttributesController(IJsonFieldsSerializer jsonFieldsSerializer, 
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
        /// Receive a list of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductSpecificationAttributes(ProductSpecifcationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var productSpecificationAttribtues = _specificationAttributeApiService.GetProductSpecificationAttributes(productId: parameters.ProductId, specificationAttributeOptionId: parameters.SpecificationAttributeOptionId, allowFiltering: parameters.AllowFiltering, showOnProductPage: parameters.ShowOnProductPage, limit: parameters.Limit, page: parameters.Page, sinceId: parameters.SinceId);

            var productSpecificationAttributeDtos = productSpecificationAttribtues.Select(x => _dtoHelper.PrepareProductSpecificationAttributeDto(x)).ToList();

            var productSpecificationAttributesRootObject = new ProductSpecificationAttributesRootObjectDto()
            {
                ProductSpecificationAttributes = productSpecificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/count")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]        
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductSpecificationAttributesCountAsync(ProductSpecifcationAttributesCountParametersModel parameters)
        {
            var productSpecificationAttributesCount = await _specificationAttributeService.GetProductSpecificationAttributeCountAsync(productId: parameters.ProductId, specificationAttributeOptionId: parameters.SpecificationAttributeOptionId);

            var productSpecificationAttributesCountRootObject = new ProductSpecificationAttributesCountRootObject()
            {
                Count = productSpecificationAttributesCount
            };

            return Ok(productSpecificationAttributesCountRootObject);
        }

        /// <summary>
        /// Retrieve product specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the product specification  attribute</param>
        /// <param name="fields">Fields from the product specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductSpecificationAttributeByIdAsync(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(id);

            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificationAttributesRootObject = new ProductSpecificationAttributesRootObjectDto();
            productSpecificationAttributesRootObject.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/productspecificationattributes")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductSpecificationAttributeAsync([ModelBinder(typeof(JsonModelBinder<ProductSpecificationAttributeDto>))] Delta<ProductSpecificationAttributeDto> productSpecificaitonAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var productSpecificationAttribute = new ProductSpecificationAttribute();
            productSpecificaitonAttributeDelta.Merge(productSpecificationAttribute);

            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);

           await  CustomerActivityService.InsertActivityAsync("AddNewProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificationAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificationAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateProductSpecificationAttributeAsync([ModelBinder(typeof(JsonModelBinder<ProductSpecificationAttributeDto>))] Delta<ProductSpecificationAttributeDto> productSpecificationAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            int productSpecificationAttributeId = productSpecificationAttributeDelta.Dto.Id;

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            productSpecificationAttributeDelta.Merge(productSpecificationAttribute);

            await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(productSpecificationAttribute);
          
            await CustomerActivityService.InsertActivityAsync("EditProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product attribute
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificatoinAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificatoinAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> DeleteProductSpecificationAttributeAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(id);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(productSpecificationAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProductSpecificationAttribute",await  LocalizationService.GetResourceAsync("ActivityLog.DeleteProductSpecificationAttribute"), productSpecificationAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}