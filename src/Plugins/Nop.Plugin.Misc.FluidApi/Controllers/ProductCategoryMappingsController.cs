using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.ProductCategoryMappings;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.MappingExtensions;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    public class ProductCategoryMappingsController : BaseApiController
    {
        private readonly IProductCategoryMappingsApiService _productCategoryMappingsService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly IProductApiService _productApiService;

        public ProductCategoryMappingsController(IProductCategoryMappingsApiService productCategoryMappingsService,
            ICategoryService categoryService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, 
            ICategoryApiService categoryApiService, 
            IProductApiService productApiService,
            IPictureService pictureService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService,pictureService)
        {
            _productCategoryMappingsService = productCategoryMappingsService;
            _categoryService = categoryService;
            _categoryApiService = categoryApiService;
            _productApiService = productApiService;
        }

        /// <summary>
        /// Receive a list of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappings(ProductCategoryMappingsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ProductCategoryMappingDto> mappingsAsDtos =
                _productCategoryMappingsService.GetMappings(parameters.ProductId,
                    parameters.CategoryId,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productCategoryMappingRootObject = new ProductCategoryMappingsRootObject()
            {
                ProductCategoryMappingDtos = mappingsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productCategoryMappingRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings/count")]
        [ProducesResponseType(typeof(ProductCategoryMappingsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingsCount(ProductCategoryMappingsCountParametersModel parameters)
        {
            if (parameters.ProductId < 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_id", "invalid product_id");
            }

            if (parameters.CategoryId < 0)
            {
                return Error(HttpStatusCode.BadRequest, "category_id", "invalid category_id");
            }

            var mappingsCount = _productCategoryMappingsService.GetMappingsCount(parameters.ProductId,
                parameters.CategoryId);

            var productCategoryMappingsCountRootObject = new ProductCategoryMappingsCountRootObject()
            {
                Count = mappingsCount
            };

            return Ok(productCategoryMappingsCountRootObject);
        }

        /// <summary>
        /// Retrieve Product-Category mappings by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the Product-Category mapping</param>
        /// <param name="fields">Fields from the Product-Category mapping you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var mapping = _productCategoryMappingsService.GetById(id);

            if (mapping == null)
            {
                return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");
            }

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();
            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(mapping.ToDto());

            var json = JsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/product_category_mappings")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProductCategoryMapping(ProductCategoryMappingDto productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var category = _categoryApiService.GetCategoryById(productCategoryDelta.CategoryId.Value);
            if (category == null)
            {
                return Error(HttpStatusCode.NotFound, "category_id", "not found");
            }

            var product = _productApiService.GetProductById(productCategoryDelta.ProductId.Value);
            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product_id", "not found");
            }

            var mappingsCount = _productCategoryMappingsService.GetMappingsCount(product.Id, category.Id);

            if (mappingsCount > 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_category_mapping", "already exist");
            }

            var newProductCategory = new ProductCategory();
            productCategoryDelta.Merge(ref newProductCategory);

            //inserting new category
            await _categoryService.InsertProductCategoryAsync(newProductCategory);

            // Preparing the result dto of the new product category mapping
            var newProductCategoryMappingDto = newProductCategory.ToDto();

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();

            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(newProductCategoryMappingDto);

            var json = JsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, string.Empty);

            //activity log 
            await CustomerActivityService.InsertActivityAsync("AddNewProductCategoryMapping",await  LocalizationService.GetResourceAsync("ActivityLog.AddNewProductCategoryMapping"), newProductCategory);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProductCategoryMappingAsync([ModelBinder(typeof(JsonModelBinder<ProductCategoryMappingDto>))] Delta<ProductCategoryMappingDto> productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (productCategoryDelta.Dto.CategoryId.HasValue)
            {
                var category = _categoryApiService.GetCategoryById(productCategoryDelta.Dto.CategoryId.Value);
                if (category == null)
                {
                    return Error(HttpStatusCode.NotFound, "category_id", "not found");
                }
            }

            if (productCategoryDelta.Dto.ProductId.HasValue)
            {
                var product = _productApiService.GetProductById(productCategoryDelta.Dto.ProductId.Value);
                if (product == null)
                {
                    return Error(HttpStatusCode.NotFound, "product_id", "not found");
                }
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            var updateProductCategoryId = productCategoryDelta.Dto.Id;

            var productCategoryEntityToUpdate = await _categoryService.GetProductCategoryByIdAsync(updateProductCategoryId);

            if (productCategoryEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");
            }

            productCategoryDelta.Merge(productCategoryEntityToUpdate);

            await _categoryService.UpdateProductCategoryAsync(productCategoryEntityToUpdate);

            //activity log
            await CustomerActivityService.InsertActivityAsync("UpdateProdutCategoryMapping", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProdutCategoryMapping"), productCategoryEntityToUpdate);

            var updatedProductCategoryDto = productCategoryEntityToUpdate.ToDto();

            var productCategoriesRootObject = new ProductCategoryMappingsRootObject();

            productCategoriesRootObject.ProductCategoryMappingDtos.Add(updatedProductCategoryDto);

            var json = JsonFieldsSerializer.Serialize(productCategoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteProductCategoryMappingAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            IList<ProductCategoryMappingDto> mappingsAsDtos =
                _productCategoryMappingsService.GetMappings(id).Select(x => x.ToDto()).ToList();

            if (mappingsAsDtos.Count() == 0)
            {
                return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");
            }

            foreach (ProductCategoryMappingDto productCategoryMappingDto in mappingsAsDtos)
            {
                var productCategory = await _categoryService.GetProductCategoryByIdAsync(productCategoryMappingDto.Id);
                await _categoryService.DeleteProductCategoryAsync(productCategory);
                //activity log 
                await CustomerActivityService.InsertActivityAsync("DeleteProductCategoryMapping",await  LocalizationService.GetResourceAsync("ActivityLog.DeleteProductCategoryMapping"), productCategory);
            }


            //var productCategory = _categoryService.GetProductCategoryByIdAsync(id);

            //if (productCategory == null)
            //{
            //    return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");
            //}

            //_categoryService.DeleteProductCategoryAsync(productCategory);

            ////activity log 
            //CustomerActivityService.InsertActivityAsync("DeleteProductCategoryMapping", LocalizationService.GetResourceAsync("ActivityLog.DeleteProductCategoryMapping"), productCategory);

            return new RawJsonActionResult("{}");
        }
    }
}