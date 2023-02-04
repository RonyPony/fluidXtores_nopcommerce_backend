﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.Images;
using Nop.Plugin.Misc.FluidApi.DTO.Manufacturers;
using Nop.Plugin.Misc.FluidApi.Factories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.ManufacturersParameters;
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
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    public class ManufacturersController : BaseApiController
    {
        private readonly IFactory<Manufacturer> _factory;
        private readonly IDTOHelper _dtoHelper;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IUrlRecordService _urlRecordService;

        public ManufacturersController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IDTOHelper dtoHelper,
            IManufacturerService manufacturerService,
            IManufacturerApiService manufacturerApiService,
            IUrlRecordService urlRecordService,
            IFactory<Manufacturer> factory)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _dtoHelper = dtoHelper;
            _manufacturerService = manufacturerService;
            _manufacturerApiService = manufacturerApiService;
            _urlRecordService = urlRecordService;
            _factory = factory;
        }

        /// <summary>
        /// Receive a list of all manufacturers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturers(ManufacturersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            var allManufacturers = new List<Manufacturer>();// _manufacturerApiService.GetManufacturers(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                            //                          parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                            //                          parameters.Limit, parameters.Page, parameters.SinceId,
                                                            //                          parameters.ProductId, parameters.PublishedStatus, parameters.LanguageId)
                                                            //.Where(async c => await StoreMappingService.AuthorizeAsync(c));

            IList<ManufacturerDto> manufacturersAsDtos = (IList<ManufacturerDto>)allManufacturers.Select(manufacturer => _dtoHelper.PrepareManufacturerDto(manufacturer)).ToList();

            var manufacturersRootObject = new ManufacturersRootObject()
            {
                Manufacturers = manufacturersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Manufacturers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/count")]
        [ProducesResponseType(typeof(ManufacturersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturersCount(ManufacturersCountParametersModel parameters)
        {
            var allManufacturersCount = _manufacturerApiService.GetManufacturersCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var manufacturersCountRootObject = new ManufacturersCountRootObject()
            {
                Count = allManufacturersCount
            };

            return Ok(manufacturersCountRootObject);
        }

        /// <summary>
        /// Retrieve manufacturer by spcified id
        /// </summary>
        /// <param name="id">Id of the manufacturer</param>
        /// <param name="fields">Fields from the manufacturer you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/{id}")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetManufacturerByIdAsync(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var manufacturer = _manufacturerApiService.GetManufacturerById(id);

            if (manufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer", "manufacturer not found");
            }

            var manufacturerDto = await _dtoHelper.PrepareManufacturerDto(manufacturer);

            var manufacturersRootObject = new ManufacturersRootObject();

            manufacturersRootObject.Manufacturers.Add( manufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateManufacturerAsync([ModelBinder(typeof(JsonModelBinder<ManufacturerDto>))] Delta<ManufacturerDto> manufacturerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the manufacturerDelta object won't be null for sure so we don't need to check for this.

            Picture insertedPicture = null;

            // We need to insert the picture before the manufacturer so we can obtain the picture id and map it to the manufacturer.
            if (manufacturerDelta.Dto.Image != null && manufacturerDelta.Dto.Image.Binary != null)
            {
                insertedPicture = await PictureService.InsertPictureAsync(manufacturerDelta.Dto.Image.Binary, manufacturerDelta.Dto.Image.MimeType, string.Empty);
            }

            // Inserting the new manufacturer
            var manufacturer = _factory.Initialize();
            manufacturerDelta.Merge(manufacturer);

            if (insertedPicture != null)
            {
                 //manufacturer.PictureId = insertedPicture.Id;
            }

            _manufacturerService.InsertManufacturerAsync(manufacturer);

            UpdateAclRoles(manufacturer, manufacturerDelta.Dto.RoleIds);

            UpdateDiscounts(manufacturer, manufacturerDelta.Dto.DiscountIds);

            UpdateStoreMappings(manufacturer, manufacturerDelta.Dto.StoreIds);

            //search engine name
            var seName = _urlRecordService.ValidateSeName(manufacturer, manufacturerDelta.Dto.SeName, manufacturer.Name, true);
            _urlRecordService.SaveSlug(manufacturer, seName, 0);
            
            CustomerActivityService.InsertActivity("AddNewManufacturer", LocalizationService.GetResourceAsync("ActivityLog.AddNewManufacturer"), manufacturer);

            // Preparing the result dto of the new manufacturer
            var newManufacturerDto = _dtoHelper.PrepareManufacturerDto(manufacturer);

            var manufacturersRootObject = new ManufacturersRootObject();

            manufacturersRootObject.Manufacturers.Add(newManufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/manufacturers/{id}")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateManufacturer([ModelBinder(typeof(JsonModelBinder<ManufacturerDto>))] Delta<ManufacturerDto> manufacturerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var manufacturer = _manufacturerApiService.GetManufacturerById(manufacturerDelta.Dto.Id);

            if (manufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer", "manufacturer not found");
            }

            manufacturerDelta.Merge(manufacturer);

            manufacturer.UpdatedOnUtc = DateTime.UtcNow;

            _manufacturerService.UpdateManufacturer(manufacturer);

            UpdatePicture(manufacturer, manufacturerDelta.Dto.Image);

            UpdateAclRoles(manufacturer, manufacturerDelta.Dto.RoleIds);

            UpdateDiscounts(manufacturer, manufacturerDelta.Dto.DiscountIds);

            UpdateStoreMappings(manufacturer, manufacturerDelta.Dto.StoreIds);

            //search engine name
            if (manufacturerDelta.Dto.SeName != null)
            {

                var seName = _urlRecordService.ValidateSeName(manufacturer, manufacturerDelta.Dto.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, seName, 0);
            }

            _manufacturerService.UpdateManufacturer(manufacturer);

            CustomerActivityService.InsertActivity("UpdateManufacturer",
                LocalizationService.GetResourceAsync("ActivityLog.UpdateManufacturer"), manufacturer);

            var manufacturerDto = _dtoHelper.PrepareManufacturerDto(manufacturer);

            var manufacturersRootObject = new ManufacturersRootObject();

            manufacturersRootObject.Manufacturers.Add(manufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/manufacturers/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteManufacturer(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var manufacturerToDelete = _manufacturerApiService.GetManufacturerById(id);

            if (manufacturerToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer", "manufacturer not found");
            }

            _manufacturerService.DeleteManufacturer(manufacturerToDelete);

            //activity log
            CustomerActivityService.InsertActivity("DeleteManufacturer", LocalizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturerToDelete);

            return new RawJsonActionResult("{}");
        }

        private void UpdatePicture(Manufacturer manufacturerEntityToUpdate, ImageDto imageDto)
        {
            // no image specified then do nothing
            if (imageDto == null)
                return;

            Picture updatedPicture;
            var currentManufacturerPicture = PictureService.GetPictureById(manufacturerEntityToUpdate.PictureId);

            // when there is a picture set for the manufacturer
            if (currentManufacturerPicture != null)
            {
                PictureService.DeletePicture(currentManufacturerPicture);

                // When the image attachment is null or empty.
                if (imageDto.Binary == null)
                {
                    manufacturerEntityToUpdate.PictureId = 0;
                }
                else
                {
                    updatedPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    manufacturerEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
            // when there isn't a picture set for the manufacturer
            else
            {
                if (imageDto.Binary != null)
                {
                    updatedPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    manufacturerEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
        }

        private void UpdateDiscounts(Manufacturer manufacturer, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
            {
                return;
            }

            var allDiscounts = DiscountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
            foreach (var discount in allDiscounts)
            {
                var appliedDiscounts = DiscountService.GetAppliedDiscounts(manufacturer);
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (appliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                    {
                        _manufacturerService.InsertDiscountManufacturerMapping(
                            new DiscountManufacturerMapping
                            {
                                DiscountId = discount.Id,
                                EntityId = manufacturer.Id
                            });
                    }
                }
                else
                {
                    //remove discount
                    if (appliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                    {
                        _manufacturerService.DeleteDiscountManufacturerMapping(
                            new DiscountManufacturerMapping
                            {
                                DiscountId = discount.Id,
                                EntityId = manufacturer.Id
                            });

                    }
                }
            }
            _manufacturerService.UpdateManufacturer(manufacturer);
        }
    }
}