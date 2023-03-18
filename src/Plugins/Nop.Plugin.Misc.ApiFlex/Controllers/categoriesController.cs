using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.ApiFlex.DTO.Errors;
using Nop.Plugin.Misc.ApiFlex.JSON.ActionResults;
using Nop.Plugin.Misc.ApiFlex.JSON.Serializers;
using Nop.Plugin.Misc.ApiFlex.Services;
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
using static Nop.Plugin.Misc.ApiFlex.Infrastructure.Constants;
using DocumentFormat.OpenXml.InkML;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Plugin.Misc.ApiFlex.Models.CategoriesParameters;
using Nop.Plugin.Misc.ApiFlex.DTO.Categories;
using Nop.Plugin.Misc.ApiFlex.Attributes;
using Nop.Plugin.Misc.ApiFlex.DTO.CustomerRoles;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nop.Plugin.Misc.ApiFlex.Controllers
{
    public class CategoriesController :ControllerBase
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;
        private readonly IDiscountService _discountService;

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IDiscountService discountService,
        IAclService aclService,
            IStoreContext storeContext,
            ICustomerService customerService) 
        {
            _categoryApiService = categoryApiService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _jsonFieldsSerializer
                = jsonFieldsSerializer;
            _discountService = discountService;
        }

        /// <summary>
        /// Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CustomerRolesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return BadRequest("Invalid page parameter");
            }

            var store = await _storeContext.GetCurrentStoreAsync();


            var allCategories = _categoryApiService.GetCategories(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);
            //.Where(c => StoreMappingService.Authorize(c, store.Id));

            //IList<CategoryDto> categoriesAsDtos = allCategories.Select(category => await _dtoHelper.PrepareCategoryDTOAsync(category));
            IList<CategoryDto> categoriesAsDtos = new List<CategoryDto>();
            foreach (Category item in allCategories)
            {
                CategoryDto cdto = new CategoryDto ();
                cdto.Id = item.Id;
                cdto.Name = item.Name;  
                cdto.Description = item.Description;
                cdto.CategoryTemplateId= item.CategoryTemplateId;
                cdto.MetaDescription= item.MetaDescription;
                cdto.Deleted = item.Deleted;
                cdto.AllowCustomersToSelectPageSize = item.AllowCustomersToSelectPageSize;
                cdto.CategoryTemplateId = item.CategoryTemplateId;
                cdto.CreatedOnUtc = item.CreatedOnUtc;
                cdto.DisplayOrder = item.DisplayOrder;
                cdto.IncludeInTopMenu = item.IncludeInTopMenu;
                cdto.MetaKeywords = item.MetaKeywords;
                cdto.MetaTitle = item.MetaTitle;
                cdto.PageSize = item.PageSize;
                cdto.PageSizeOptions = item.PageSizeOptions;    
                cdto.ParentCategoryId = item.ParentCategoryId;
                cdto.UpdatedOnUtc = item.UpdatedOnUtc;
                cdto.Published = item.Published;
                cdto.ShowOnHomePage = item.ShowOnHomepage;
                

                categoriesAsDtos.Add(cdto);
            }


            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };
            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/count")]
        [ProducesResponseType(typeof(CategoriesCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            var allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject()
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        /// Retrieve category by spcified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //[GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetCategoryById(int id, string fields = "")
        {
            if (id <= 0)
            {
                //return Error(HttpStatusCode.BadRequest, "id", "invalid id");
                return BadRequest("Invalid ID");
            }

            var category = _categoryApiService.GetCategoryById(id);

            if (category == null)
            {
                //return Error(HttpStatusCode.NotFound, "category", "category not found");
                return NotFound("Category not found");
            }

            CategoryDto cdto = new CategoryDto();
            cdto.Id = category.Id;
            cdto.Name = category.Name;
            cdto.Description = category.Description;
            cdto.CategoryTemplateId = category.CategoryTemplateId;
            cdto.MetaDescription = category.MetaDescription;
            cdto.Deleted = category.Deleted;
            cdto.AllowCustomersToSelectPageSize = category.AllowCustomersToSelectPageSize;
            cdto.CategoryTemplateId = category.CategoryTemplateId;
            cdto.CreatedOnUtc = category.CreatedOnUtc;
            cdto.DisplayOrder = category.DisplayOrder;
            cdto.IncludeInTopMenu = category.IncludeInTopMenu;
            cdto.MetaKeywords = category.MetaKeywords;
            cdto.MetaTitle = category.MetaTitle;
            cdto.PageSize = category.PageSize;
            cdto.PageSizeOptions = category.PageSizeOptions;
            cdto.ParentCategoryId = category.ParentCategoryId;
            cdto.UpdatedOnUtc = category.UpdatedOnUtc;
            cdto.Published = category.Published;
            cdto.ShowOnHomePage = category.ShowOnHomepage;



            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(cdto);
            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateCategoryAsync(CategoryDto categoryDelta)
        {


            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.

            Picture insertedPicture = null;

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            //if (categoryDelta.Image != null && categoryDelta.Image.Binary != null)
            //{
            //    insertedPicture = await PictureService.InsertPictureAsync(categoryDelta.Image.Binary, categoryDelta.Image.MimeType, string.Empty);
            //}

            // Inserting the new category
            var category = new Category();// await _factory.Initialize();
            categoryDelta.Merge(ref category);

            if (insertedPicture != null)
            {
                category.PictureId = insertedPicture.Id;
            }

            await _categoryService.InsertCategoryAsync(category);


            //await UpdateAclRolesAsync(category, categoryDelta.RoleIds);

            UpdateDiscountsAsync(category, categoryDelta.DiscountIds);

            //UpdateStoreMappings(category, categoryDelta.StoreIds);

            var seName = await _urlRecordService.ValidateSeNameAsync(category, categoryDelta.SeName, category.Name, true);
            await _urlRecordService.SaveSlugAsync(category, seName, 0);
            await _customerActivityService.InsertActivityAsync("AddNewCategory", await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category);

            // Preparing the result dto of the new category
            var newCategoryDto = new CategoryDto();//await _dtoHelper.PrepareCategoryDTOAsync(category);
            newCategoryDto.Id = category.Id;
            newCategoryDto.Name = category.Name;
            newCategoryDto.Description = category.Description;

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCategoryAsync(int id)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }

            var category =  _categoryApiService.GetCategoryById(id);

            if (category == null)
            {
                //return Error(HttpStatusCode.NotFound, "category", "category not found");
                return NotFound("Category not found");
            }

            //categoryDelta.Merge(category);

            category.UpdatedOnUtc = DateTime.UtcNow;

            await _categoryService.UpdateCategoryAsync(category);

            //UpdatePictureAsync(category, categoryDelta.Dto.Image);

            //UpdateAclRolesAsync(category, categoryDelta.Dto.RoleIds);

            //UpdateDiscountsAsync(category, categoryDelta.Dto.DiscountIds);

            //UpdateStoreMappings(category, categoryDelta.Dto.StoreIds);

            //search engine name
            //if (categoryDelta.Dto.SeName != null)
            //{

            //var seName = await _urlRecordService.ValidateSeNameAsync(category, categoryDelta.Dto.SeName, category.Name, true);
            //await _urlRecordService.SaveSlugAsync(category, seName, 0);
            //}

            await _categoryService.UpdateCategoryAsync(category);

            //await CustomerActivityService.InsertActivityAsync("UpdateCategory",
            //    await LocalizationService.GetResourceAsync("ActivityLog.UpdateCategory"), category);

            var categoryDto = new CategoryDto();// await _dtoHelper.PrepareCategoryDTOAsync(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                //return Error(HttpStatusCode.BadRequest, "id", "invalid id");
                return BadRequest("Not valid ID");
            }

            var categoryToDelete = _categoryApiService.GetCategoryById(id);

            if (categoryToDelete == null)
            {
                //return Error(HttpStatusCode.NotFound, "category", "category not found");
                return NotFound("Category not found");
            }

            await _categoryService.DeleteCategoryAsync(categoryToDelete);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCategory", await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), categoryToDelete);

            return new RawJsonActionResult("{}");
        }

        private async Task UpdatePictureAsync(Category categoryEntityToUpdate, int imageDto)
        {
            // no image specified then do nothing
            if (imageDto == null)
                return;

            //Picture updatedPicture;
            //var currentCategoryPicture = await PictureService.GetPictureByIdAsync(categoryEntityToUpdate.PictureId);

            //// when there is a picture set for the category
            //if (currentCategoryPicture != null)
            //{
            //    await PictureService.DeletePictureAsync(currentCategoryPicture);

            //    // When the image attachment is null or empty.
            //    if (imageDto.Binary == null)
            //    {
            //        categoryEntityToUpdate.PictureId = 0;
            //    }
            //    else
            //    {
            //        updatedPicture = await PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
            //        categoryEntityToUpdate.PictureId = updatedPicture.Id;
            //    }
            //}
            //// when there isn't a picture set for the category
            //else
            //{
            //    if (imageDto.Binary != null)
            //    {
            //        updatedPicture = await PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
            //        categoryEntityToUpdate.PictureId = updatedPicture.Id;
            //    }
            //}
        }

        private async Task UpdateDiscountsAsync(Category category, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
            {
                return;
            }

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
            var appliedCategoryDiscount = await _discountService.GetAppliedDiscountsAsync(category);
            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (appliedCategoryDiscount.Count(d => d.Id == discount.Id) == 0)
                    {
                        appliedCategoryDiscount.Add(discount);
                    }
                }
                else
                {
                    //remove discount
                    if (appliedCategoryDiscount.Count(d => d.Id == discount.Id) > 0)
                    {
                        appliedCategoryDiscount.Remove(discount);
                    }
                }
            }
            await _categoryService.UpdateCategoryAsync(category);
        }
    }
}