using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.Images;
using Nop.Plugin.Misc.FluidApi.DTO.ProductImages;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Net;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    public class ProductPicturesController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IDTOHelper _dtoHelper;

        //private readonly IFactory<Picture> _factory;

        public ProductPicturesController(IJsonFieldsSerializer jsonFieldsSerializer,
                                        IAclService aclService,
                                        ICustomerService customerService,
                                        IStoreMappingService storeMappingService,
                                        IStoreService storeService,
                                        IDiscountService discountService,
                                        ICustomerActivityService customerActivityService,
                                        ILocalizationService localizationService,
                                        IPictureService pictureService,
                                        IProductService productService,
                                        IDTOHelper dtoHelper) : 
            base(jsonFieldsSerializer, 
                aclService, 
                customerService, 
                storeMappingService, 
                storeService,
                discountService, 
                customerActivityService, 
                localizationService, 
                pictureService)
        {
            _productService = productService;
            _dtoHelper = dtoHelper;
        }

        [HttpPut]
        [Route("/api/productpictures/{id}")]
        [ProducesResponseType(typeof(ProductPicturesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateProductPicture([ModelBinder(typeof(JsonModelBinder<ImageMappingDto>))] Delta<ImageMappingDto> productPictureDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            throw new NotImplementedException("Update is not yet implemented.");

            /*
                Because there appears to be a bug in the PictureService.UpdatePicture method (where the thumbnail images aren't replaced) we have not implemented this method.
                The below code *should* work but it doesn't because it appears that the thumbnail images arent being updated in the thumbnail store.
                Leaving the code here so that it might be addressed at some point.
            */

            
            //var product = _productService.GetProductById(productPictureDelta.Dto.ProductId);
            //var picture = PictureService.GetPictureById(productPictureDelta.Dto.Id);

            //var updatedPicture = PictureService.UpdatePicture(picture.Id, productPictureDelta.Dto.Binary, productPictureDelta.Dto.MimeType, productPictureDelta.Dto.SeoFilename);
            //PictureService.SetSeoFilename(picture.Id, PictureService.GetPictureSeName(product.Name));

            //var productPicture = _productService.GetProductPictureById(productPictureDelta.Dto.Id);
            //productPicture.DisplayOrder = productPictureDelta.Dto.Position;

            //_productService.UpdateProductPicture(productPicture);

            //var productImagesRootObject = new ProductPicturesRootObjectDto();
            //productImagesRootObject.Image = _dtoHelper.PrepareProductPictureDTO(productPicture);

            //var json = JsonFieldsSerializer.Serialize(productImagesRootObject, string.Empty);

            //return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/productpictures")]
        [ProducesResponseType(typeof(ProductPicturesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductPictureAsync([ModelBinder(typeof(JsonModelBinder<ImageMappingDto>))] Delta<ImageMappingDto> productPictureDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            await CustomerActivityService.InsertActivityAsync("APIService", string.Format("Attempting to create picture with name {0}.", productPictureDelta.Dto.SeoFilename), null);

            var newPicture = await PictureService.InsertPictureAsync(Convert.FromBase64String(productPictureDelta.Dto.Attachment), productPictureDelta.Dto.MimeType, productPictureDelta.Dto.SeoFilename);

            var productPicture = new ProductPicture()
            {
                PictureId = newPicture.Id,
                ProductId = productPictureDelta.Dto.ProductId,
                DisplayOrder = productPictureDelta.Dto.Position
            };

            await _productService.InsertProductPictureAsync(productPicture);

            var product = await _productService.GetProductByIdAsync(productPictureDelta.Dto.ProductId);
            await PictureService.SetSeoFilenameAsync(newPicture.Id, await PictureService.GetPictureSeNameAsync(product.Name));
            
            var productImagesRootObject = new ProductPicturesRootObjectDto();
            productImagesRootObject.Image = await _dtoHelper.PrepareProductPictureDTO(productPicture);
            
            var json = JsonFieldsSerializer.Serialize(productImagesRootObject, string.Empty);

            await CustomerActivityService.InsertActivityAsync("APIService", string.Format("Successfully created and returned image {0}.", productPictureDelta.Dto.SeoFilename), null);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productpictures/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeletePictureAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            await CustomerActivityService.InsertActivityAsync("APIService", string.Format("Attempting delete of picture {0}.", id), null);

            var productPicture = await _productService.GetProductPictureByIdAsync(id);
            if (productPicture == null)
                return Error(HttpStatusCode.NotFound, "product picture", "not found");

            var picture = await PictureService.GetPictureByIdAsync(productPicture.PictureId);
            if (picture == null)
                return Error(HttpStatusCode.NotFound, "picture", "not found");


            await _productService.DeleteProductPictureAsync(productPicture);
            await PictureService.DeletePictureAsync(picture);
            

            await CustomerActivityService.InsertActivityAsync("APIService", string.Format("Deleted picture {0}.", picture.Id), picture);

            return new RawJsonActionResult("{}");
        }

    }
}
