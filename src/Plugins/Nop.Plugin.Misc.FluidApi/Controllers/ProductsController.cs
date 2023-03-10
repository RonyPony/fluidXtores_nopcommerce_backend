using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.Images;
using Nop.Plugin.Misc.FluidApi.DTO.Products;
using Nop.Plugin.Misc.FluidApi.Factories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.ProductsParameters;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    [AllowAnonymous]
    public class ProductsController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IManufacturerService _manufacturerService;
        private readonly Factory<Product> _factory;
        private readonly IProductTagService _productTagService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDTOHelper _dtoHelper;
        private readonly ILogger _logger;

        public ProductsController(IProductApiService productApiService,
                                  IJsonFieldsSerializer jsonFieldsSerializer,
                                  IProductService productService,
                                  IUrlRecordService urlRecordService,
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  Factory<Product> factory,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IManufacturerService manufacturerService,
                                  IProductTagService productTagService,
                                  IProductAttributeService productAttributeService,
                                  ILogger logger,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _logger = logger;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        [AllowAnonymous]
        public IActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            //var allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
            //                                                            parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
            //                                                            parameters.VendorName, parameters.PublishedStatus)
            //                                    .Where(async p => await StoreMappingService.AuthorizeAsync(p));

            //IList<ProductDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductDTOAsync(product)).ToList();

            //var productsRootObject = new ProductsRootObjectDto()
            //{
            //    Products = productsAsDtos
            //};

            //var json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            //return new RawJsonActionResult(json);
            return Ok("Error");
        }

        /// <summary>
        /// Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/count")]
        [ProducesResponseType(typeof(ProductsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        [AllowAnonymous]
        public IActionResult GetProductsCount(ProductsCountParametersModel parameters)
        {
            var allProductsCount = _productApiService.GetProductsCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                       parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName,
                                                                       parameters.CategoryId);

            var productsCountRootObject = new ProductsCountRootObject()
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        /// <summary>
        /// Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductByIdAsync(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductAsync( ProductDto productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            await CustomerActivityService.InsertActivityAsync("APIService", "Starting Product Create", null);

            // Inserting the new product
            var product = await _factory.Initialize();
            productDelta.Merge(ref product);
            
            await _productService.InsertProductAsync(product);

            UpdateProductAsyncPictures(product, productDelta.Images);

            await UpdateProductAsyncTagsAsync(product, productDelta.Tags);

            await UpdateProductAsyncManufacturersAsync(product, productDelta.ManufacturerIds);

            await UpdateAssociatedProductsAsync(product, productDelta.AssociatedProductIds);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(product, productDelta.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, seName, 0);

            await UpdateAclRolesAsync(product, productDelta.RoleIds);

            await UpdateDiscountMappingsAsync(product, productDelta.DiscountIds);

            UpdateStoreMappings(product, productDelta.StoreIds);

            await _productService.UpdateProductAsync(product);

            await CustomerActivityService.InsertActivityAsync("APIService",await LocalizationService.GetResourceAsync("ActivityLog.AddNewProduct"), product);

            // Preparing the result dto of the new product
            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);
            
            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProductAsync([FromBody] ProductDto productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }
            await CustomerActivityService.InsertActivityAsync("APIService", "Starting Product Update", null);

            var product = _productApiService.GetProductById(productDelta.Id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(ref product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.UpdateProductAsync(product);

            //UpdateProductAsyncAttributes(product, productDelta);

            UpdateProductAsyncPictures(product, productDelta.Images);

            await UpdateProductAsyncTagsAsync(product, productDelta.Tags);

            await UpdateProductAsyncManufacturersAsync(product, productDelta.ManufacturerIds);

            await UpdateAssociatedProductsAsync(product, productDelta.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.SeName != null)
            {
                var seName = await _urlRecordService.ValidateSeNameAsync(product, productDelta.SeName, product.Name, true);
                await _urlRecordService.SaveSlugAsync(product, seName, 0);
            }

            await UpdateDiscountMappingsAsync(product, productDelta.DiscountIds);

            UpdateStoreMappings(product, productDelta.StoreIds);

            await UpdateAclRolesAsync(product, productDelta.RoleIds);

            await _productService.UpdateProductAsync(product);

            await CustomerActivityService.InsertActivityAsync("APIService", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProductAsync"), product);

            // Preparing the result dto of the new product
            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
            
            
        }


        [HttpPost]
        [Route("/api/UpdateExistProduct")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateExistProductAsync([FromBody] ProductDto productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }
            await CustomerActivityService.InsertActivityAsync("APIService", "Starting Product Update", null);

            var product = _productApiService.GetProductById(productDelta.Id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(ref product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.UpdateProductAsync(product);

            //UpdateProductAsyncAttributes(product, productDelta);

            UpdateProductAsyncPictures(product, productDelta.Images);

            await UpdateProductAsyncTagsAsync(product, productDelta.Tags);

            await UpdateProductAsyncManufacturersAsync(product, productDelta.ManufacturerIds);

            await UpdateAssociatedProductsAsync(product, productDelta.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.SeName != null)
            {
                var seName = await _urlRecordService.ValidateSeNameAsync(product, productDelta.SeName, product.Name, true);
                await _urlRecordService.SaveSlugAsync(product, seName, 0);
            }

            await UpdateDiscountMappingsAsync(product, productDelta.DiscountIds);

            UpdateStoreMappings(product, productDelta.StoreIds);

            await UpdateAclRolesAsync(product, productDelta.RoleIds);

            await _productService.UpdateProductAsync(product);

            await CustomerActivityService.InsertActivityAsync("APIService", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProductAsync"), product);

            // Preparing the result dto of the new product
            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);


        }



        [HttpDelete]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            await _productService.DeleteProductAsync(product);

            //activity log
            await CustomerActivityService.InsertActivityAsync("APIService", string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductAsync"), product.Name), product);

            return new RawJsonActionResult("{}");
        }

        private async void UpdateProductAsyncPictures(Product entityToUpdate, List<ImageMappingDto> setPictures)
        {
            // If no pictures are specified means we don't have to update anything
            if (setPictures == null)
            {
                return;
            }

            // delete unused product pictures
            var productPictures =await  _productService.GetProductPicturesByProductIdAsync(entityToUpdate.Id);
            var unusedProductPictures = productPictures.Where(x => setPictures.All(y => y.Id != x.Id)).ToList();
            foreach (var unusedProductPicture in unusedProductPictures)
            {
                var picture = await PictureService.GetPictureByIdAsync(unusedProductPicture.PictureId);
                if (picture == null)
                {
                    throw new ArgumentException("No picture found with the specified id");
                }
                await PictureService.DeletePictureAsync(picture);
            }

            foreach (var imageDto in setPictures)
            {
                if (imageDto.Id > 0)
                {
                    // update existing product picture
                    var productPictureToUpdate = productPictures.FirstOrDefault(x => x.Id == imageDto.Id);
                    if (productPictureToUpdate != null && imageDto.Position > 0)
                    {
                        productPictureToUpdate.DisplayOrder = imageDto.Position;
                        await _productService.UpdateProductPictureAsync(productPictureToUpdate);
                    }
                }
                else
                {
                    // add new product picture
                    var newPicture = PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        PictureId = newPicture.Id,
                        ProductId = entityToUpdate.Id,
                        DisplayOrder = imageDto.Position
                    });
                }
            }
        }
        private async Task UpdateProductAsyncAttributesAsync(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductAttributeMappings == null)
            {
                return;
            }

            // delete unused product attribute mappings
            var toBeUpdatedIds = productDtoDelta.Dto.ProductAttributeMappings.Where(y => y.Id != 0).Select(x => x.Id);
            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(entityToUpdate.Id);
            var unusedProductAttributeMappings = productAttributeMappings.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeMapping in unusedProductAttributeMappings)
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(unusedProductAttributeMapping);
            }

            foreach (var productAttributeMappingDto in productDtoDelta.Dto.ProductAttributeMappings)
            {
                if (productAttributeMappingDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeMappingToUpdate = productAttributeMappings.FirstOrDefault(x => x.Id == productAttributeMappingDto.Id);
                    if (productAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeMappingDto, productAttributeMappingToUpdate, false);

                        //_productAttributeService.UpdateProductAtributeMapping(productAttributeMappingToUpdate);

                        //UpdateProductAsyncAttributeValues(productAttributeMappingDto, productDtoDelta);
                    }
                }
                else
                {
                    var newProductAttributeMapping = new ProductAttributeMapping
                    {
                        ProductId = entityToUpdate.Id
                    };

                    productDtoDelta.Merge(productAttributeMappingDto, newProductAttributeMapping);

                    // add new product attribute
                    //_productAttributeService.InsertProductAsyncAttributeMapping(newProductAttributeMapping);
                }
            }
        }

        private async void UpdateProductAttributeValues(ProductAttributeMappingDto productAttributeMappingDto, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute values are specified means we don't have to update anything
            if (productAttributeMappingDto.ProductAttributeValues == null)
                return;

            // delete unused product attribute values
            var toBeUpdatedIds = productAttributeMappingDto.ProductAttributeValues.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeValues =
               (await  _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingDto.Id)).Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeValue in unusedProductAttributeValues)
            {
                await _productAttributeService.DeleteProductAttributeValueAsync(unusedProductAttributeValue);
            }

            foreach (var productAttributeValueDto in productAttributeMappingDto.ProductAttributeValues)
            {
                if (productAttributeValueDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeValueToUpdate =
                        await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueDto.Id);
                    if (productAttributeValueToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeValueDto, productAttributeValueToUpdate, false);

                        await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValueToUpdate);
                    }
                }
                else
                {
                    var newProductAttributeValue = new ProductAttributeValue();
                    productDtoDelta.Merge(productAttributeValueDto, newProductAttributeValue);

                    newProductAttributeValue.ProductAttributeMappingId = productAttributeMappingDto.Id;
                    // add new product attribute value
                    await _productAttributeService.InsertProductAttributeValueAsync(newProductAttributeValue);
                }
            }
        }

        private async Task UpdateProductAsyncTagsAsync(Product product, IReadOnlyCollection<string> productTags)
        {
            if (productTags == null)
            {
                return;
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var existingProductTags = await _productTagService.GetAllProductTagsByProductIdAsync(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (var newProductTag in productTags)
                {
                    if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    found = true;
                    break;
                }

                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }

            try
            {
                await this._productTagService.UpdateProductTagsAsync(product, productTagsToRemove.Select(o => o.Name).ToArray());

                foreach (var productTagName in productTags)
                {
                    ProductTag productTag;
                    var productTag2 = _productTagService.GetAllProductTagsAsync(productTagName);
                    if (productTag2 == null)
                    {
                        //add new product tag
                        productTag = new ProductTag
                        {
                            Name = productTagName
                        };
                        //_productTagService.InsertProductProductTagMappingAsync(productTag);
                    }
                    else
                    {
                        //productTag = productTag2;
                    }

                    //var seName = _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
                    //_urlRecordService.SaveSlugAsync(productTag, seName, 0);

                    ////Perform a final check to deal with duplicates etc.
                    //var currentProductTags = _productTagService.GetAllProductTagsByProductId(product.Id);
                    //if (!currentProductTags.Any(o => o.Id == productTag.Id))
                    //{
                    //    _productTagService.InsertProductAsyncProductTagMapping(new ProductProductTagMapping()
                    //    {
                    //        ProductId = product.Id,
                    //        ProductTagId = productTag.Id
                    //    });
                    //}

                }
            } 
            catch (Exception ex)
            {
                throw;
            }            
        }
        private async Task UpdateDiscountMappingsAsync(Product product, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
            {
                return;
            }

            var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true);
            var appliedProductDiscount = await DiscountService.GetAppliedDiscountsAsync(product);
            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (appliedProductDiscount.Count(d => d.Id == discount.Id) == 0)
                    {
                        appliedProductDiscount.Add(discount);
                    }
                }
                else
                {
                    //remove discount
                    if (appliedProductDiscount.Count(d => d.Id == discount.Id) > 0)
                    {
                        appliedProductDiscount.Remove(discount);
                    }
                }
            }

            await _productService.UpdateProductAsync(product);
            await _productService.UpdateHasDiscountsAppliedAsync(product);
        }



        private async Task UpdateProductAsyncManufacturersAsync(Product product, List<int> passedManufacturerIds)
        {
            // If no manufacturers specified then there is nothing to map 
            if (passedManufacturerIds == null)
            {
                return;
            }
            var productmanufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
            var unusedProductManufacturers = productmanufacturers.Where(x => !passedManufacturerIds.Contains(x.Id)).ToList();

            // remove all manufacturers bindins that are not passed
            foreach (var unusedProductManufacturer in unusedProductManufacturers)
            {
                await _manufacturerService.DeleteProductManufacturerAsync(unusedProductManufacturer);
            }

            foreach (var passedManufacturerId in passedManufacturerIds)
            {
                // not part of existing manufacturers so we will create a new one
                if (productmanufacturers.All(x => x.Id != passedManufacturerId))
                {
                    // if manufacturer does not exist we simply ignore it, otherwise add it to the product
                    var manufacturer = _manufacturerService.GetManufacturerByIdAsync(passedManufacturerId);
                    if (manufacturer != null)
                    {
                        await _manufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
                        {
                            ProductId = product.Id,
                            ManufacturerId = manufacturer.Id
                        });
                    }
                }
            }
        }
        private async Task UpdateAssociatedProductsAsync(Product product, List<int> passedAssociatedProductIds)
        {
            // If no associated products specified then there is nothing to map 
            if (passedAssociatedProductIds == null)
                return;

            var noLongerAssociatedProducts =
                (await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true))
                    .Where(p => !passedAssociatedProductIds.Contains(p.Id));

            // update all products that are no longer associated with our product
            foreach (var noLongerAssocuatedProduct in noLongerAssociatedProducts)
            {
                noLongerAssocuatedProduct.ParentGroupedProductId = 0;
                await _productService.UpdateProductAsync(noLongerAssocuatedProduct);
            }

            var newAssociatedProducts = await _productService.GetProductsByIdsAsync(passedAssociatedProductIds.ToArray());
            foreach (var newAssociatedProduct in newAssociatedProducts)
            {
                newAssociatedProduct.ParentGroupedProductId = product.Id;
                await _productService.UpdateProductAsync(newAssociatedProduct);
            }
        }
    }
}