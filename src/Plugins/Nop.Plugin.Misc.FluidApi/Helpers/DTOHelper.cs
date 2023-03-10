using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Misc.FluidApi.DTO.Categories;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.Images;
using Nop.Plugin.Misc.FluidApi.DTO.Languages;
using Nop.Plugin.Misc.FluidApi.DTO.Manufacturers;
using Nop.Plugin.Misc.FluidApi.DTO.OrderItems;
using Nop.Plugin.Misc.FluidApi.DTO.Orders;
using Nop.Plugin.Misc.FluidApi.DTO.ProductAttributes;
using Nop.Plugin.Misc.FluidApi.DTO.Products;
using Nop.Plugin.Misc.FluidApi.DTO.Shipments;
using Nop.Plugin.Misc.FluidApi.DTO.ShoppingCarts;
using Nop.Plugin.Misc.FluidApi.DTO.SpecificationAttributes;
using Nop.Plugin.Misc.FluidApi.DTO.Stores;
using Nop.Plugin.Misc.FluidApi.DTO.TaxCategory;
using Nop.Plugin.Misc.FluidApi.MappingExtensions;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Helpers
{
    public class DTOHelper : IDTOHelper
    {
        private readonly CurrencySettings _currencySettings;
        private readonly IAclService _aclService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly ISettingService _settingService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IAddressService _addressService;
        private readonly IVendorService _vendorService;

        public DTOHelper(IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ICustomerApiService customerApiService,
            ICustomerService customerService,
            IProductAttributeConverter productAttributeConverter,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            CurrencySettings currencySettings,
            IStoreService storeService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IProductTagService productTagService,
            ITaxCategoryService taxCategoryService,
            ISettingService settingService,
            IShipmentService shipmentService,
            IOrderService orderService,
            IAddressService addressService, 
            IVendorService vendorService)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _customerApiService = customerApiService;
            _customerService = customerService;
            _productAttributeConverter = productAttributeConverter;
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _storeService = storeService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            _taxCategoryService = taxCategoryService;
            _settingService = settingService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _shipmentService = shipmentService;
            _addressService = addressService;
            _vendorService = vendorService;
        }

        public async Task<ProductDto> PrepareProductDTOAsync(Product product)
        {
            var productDto = product.ToDto();

            var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);
            PrepareProductImagesAsync(productPictures, productDto);


            productDto.SeName =await _urlRecordService.GetSeNameAsync(product);
            productDto.DiscountIds =(await _discountService.GetAppliedDiscountsAsync(product)).Select(discount => discount.Id).ToList();
            productDto.ManufacturerIds =(await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).Select(pm => pm.Id).ToList();
            productDto.RoleIds = (await _aclService.GetAclRecordsAsync(product)).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(product)).Select(mapping => mapping.StoreId)
                .ToList();
            productDto.Tags = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name)
                .ToList();
            //productDto.ProductAttributeMappings =(await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id)).Select(async attribute =>  new ProductAttributeMappingDto {
            //    AttributeControlType = attribute.AttributeControlType.ToString(),
            //    AttributeControlTypeId = attribute.AttributeControlTypeId,
            //    ProductAttributeId = attribute.ProductAttributeId,
            //    DefaultValue = attribute.DefaultValue,
            //    DisplayOrder = attribute.DisplayOrder,
            //    Id = attribute.Id,
            //    IsRequired = attribute.IsRequired,
            //    ProductAttributeName= (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
            //    ProductAttributeValues = (await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id)).Select(value => new ProductAttributeValueDto
            //    {
            //        AssociatedProductId = value.AssociatedProductId,
            //        AttributeValueTypeId = value.AttributeValueTypeId,
            //        AttributeValueType = value.AttributeValueType.ToString(),
            //        ColorSquaresRgb = value.ColorSquaresRgb,
            //        Cost = value.Cost,
            //        DisplayOrder = value.DisplayOrder,
            //        Id = value.Id,
            //        IsPreSelected = value.IsPreSelected,
            //        Name = value.Name,
            //        PriceAdjustment = value.PriceAdjustment,
            //        Quantity = value.Quantity,
            //        WeightAdjustment = value.WeightAdjustment,
            //    }).ToList(),
            //    TextPrompt = attribute.TextPrompt,
            //} ).ToList();

            productDto.AssociatedProductIds =
                (await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true)
                    ).Select(associatedProduct => associatedProduct.Id)
                    .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            productDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                {
                    LanguageId = language.Id,
                    LocalizedName = await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id)
                };

                productDto.LocalizedNames.Add(localizedNameDto);
            }

            return productDto;
        }
        
        protected async Task<ImageMappingDto> PrepareProductImageDtoAsync(ProductPicture productPicture)
        {
            ImageMappingDto imageMapping = null;

            var picture = await this._pictureService.GetPictureByIdAsync(productPicture.PictureId);

            if (productPicture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                imageMapping = new ImageMappingDto
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Id = productPicture.Id,
                    ProductId = productPicture.ProductId,
                    PictureId = productPicture.PictureId,
                    Position = productPicture.DisplayOrder,
                    MimeType = picture.MimeType,
                    Src =await _pictureService.GetPictureUrlAsync(productPicture.PictureId)
                };
            }

            return imageMapping;
        }
        public async Task<CategoryDto> PrepareCategoryDTOAsync(Category category)
        {
            var categoryDto = category.ToDto();

            var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
            var imageDto = PrepareImageDtoAsync(picture);

            if (imageDto != null)
            {
                categoryDto.Image = await imageDto;
            }

            categoryDto.SeName = await _urlRecordService.GetSeNameAsync(category);
            categoryDto.DiscountIds = (await _discountService.GetAppliedDiscountsAsync(category)).Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = (await _aclService.GetAclRecordsAsync(category)).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(category)).Select(mapping => mapping.StoreId)
                .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            categoryDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                {
                    LanguageId = language.Id,
                    LocalizedName = await _localizationService.GetLocalizedAsync(category, x => x.Name, language.Id)
                };

                categoryDto.LocalizedNames.Add(localizedNameDto);
            }

            return categoryDto;
        }

        public async Task<OrderDto> PrepareOrderDTOAsync(Order order)
        {
            try
            {
                var orderDto = order.ToDto();

                orderDto.OrderItems = (ICollection<OrderItemDto>)(await _orderService.GetOrderItemsAsync(order.Id)).Select(PrepareOrderItemDTOAsync).ToList();
                orderDto.Shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Select(PrepareShippingItemDTO).ToList();
                
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                orderDto.BillingAddress = billingAddress.ToDto();

                if (order.PickupInStore && order.PickupAddressId.HasValue)
                {
                    var pickUpInStoreAddress = await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value);
                    orderDto.ShippingAddress = pickUpInStoreAddress.ToDto();
                }

                if (order.ShippingAddressId.HasValue)
                {
                    var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
                    orderDto.ShippingAddress = shippingAddress.ToDto();
                }
                
                var customerDto = await _customerApiService.GetCustomerByIdAsync(order.CustomerId);

                if (customerDto != null)
                {
                    orderDto.Customer = customerDto.ToOrderCustomerDto();
                }

                int vendorId = orderDto.OrderItems.FirstOrDefault().Product.VendorId.Value;

                if (vendorId != 0)
                {
                    var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
                    orderDto.VendorName = vendor.Name;

                    if (vendor.AddressId != 0)
                    {
                        var vendorAddress = await _addressService.GetAddressByIdAsync(vendor.AddressId);
                        orderDto.VendorAddress = vendorAddress.ToDto();
                    }
                }

                orderDto.PaymentMethodCheckoutAttribute = GetPaymentMethod(orderDto.CheckoutAttributeDescription);
                orderDto.OrderDeliveryIndicationsCheckoutAttribute = GetOrderDeliveryIndications(orderDto.CheckoutAttributeDescription);

                return orderDto;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
        private string GetOrderDeliveryIndications(string checkoutAttributeDescription)
        {
            var attributes = checkoutAttributeDescription.Split("<br />");
            string orderDeliveryIndications = attributes.FirstOrDefault(x => x.Contains("OrderDeliveryIndications"));

            if (string.IsNullOrWhiteSpace(orderDeliveryIndications))
            {
                return "";
            }

            orderDeliveryIndications = orderDeliveryIndications.Replace("OrderDeliveryIndications:", "");

            return orderDeliveryIndications;
        }

        private string GetPaymentMethod(string checkoutAttributeDescription)
        {
            if (checkoutAttributeDescription.Contains("TarjetaCredito"))
            {
                return "TarjetaCredito";

            } else if (checkoutAttributeDescription.Contains("TarjetaClave"))
            {
                return "TarjetaClave";
            }
            else
            {
                return "Efectivo";
            }
        }

        public async Task<ShoppingCartItemDto> PrepareShoppingCartItemDTOAsync(ShoppingCartItem shoppingCartItem)
        {
            var dto = shoppingCartItem.ToDto();
            dto.ProductDto = await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(shoppingCartItem.ProductId));
            dto.CustomerDto = (await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId)).ToCustomerForShoppingCartItemDto();
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }

        public ShipmentDto PrepareShippingItemDTO(Shipment shipment)
        {
            return new ShipmentDto()
            {
                AdminComment = shipment.AdminComment,
                CreatedOnUtc = shipment.CreatedOnUtc,
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                ShippedDateUtc = shipment.ShippedDateUtc,
                TotalWeight = shipment.TotalWeight,
                TrackingNumber = shipment.TrackingNumber
            };

        }
        public async Task<OrderItemDto> PrepareOrderItemDTOAsync(OrderItem orderItem)
        {
            var dto = orderItem.ToDto();
            dto.Product = await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(orderItem.ProductId));
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }

        public async Task<StoreDto> PrepareStoreDTOAsync(Store store)
        {
            var storeDto = store.ToDto();

            var primaryCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale))
            {
                storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;
            }

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }

        public async Task<LanguageDto> PrepareLanguageDtoAsync(Language language)
        {
            var languageDto = language.ToDto();

            languageDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(language)).Select(mapping => mapping.StoreId)
                .ToList();

            if (languageDto.StoreIds.Count == 0)
            {
                languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();
            }

            return languageDto;
        }

        public ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute)
        {
            return productAttribute.ToDto();
        }

        private async Task PrepareProductImagesAsync(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null)
            {
                productDto.Images = new List<ImageMappingDto>();
            }

            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                var imageDto = await PrepareImageDtoAsync(await _pictureService.GetPictureByIdAsync(productPicture.PictureId));

                if (imageDto != null)
                {
                    var productImageDto = new ImageMappingDto
                    {
                        Id = productPicture.Id,
                        PictureId = productPicture.PictureId,
                        Position = productPicture.DisplayOrder,
                        Src = imageDto.Src,
                        Attachment = imageDto.Attachment
                    };

                    productDto.Images.Add(productImageDto);
                }
            }
        }

        protected async Task<ImageDto> PrepareImageDtoAsync(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
            {
                // We don't use the image from the passed dto directly 
                // because the picture may be passed with src and the result should only include the base64 format.
                image = new ImageDto
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Src = await _pictureService.GetPictureUrlAsync(picture.Id)
                };
            }

            return image;
        }


        private async void PrepareProductAttributes(IEnumerable<ProductAttributeMapping> productAttributeMappings,
            ProductDto productDto)
        {
            if (productDto.ProductAttributeMappings == null)
            {
                productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();
            }

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                var productAttributeMappingDto = await 
                    PrepareProductAttributeMappingDtoAsync(productAttributeMapping);

                if (productAttributeMappingDto != null)
                {
                    productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
                }
            }
        }

        private async Task<ProductAttributeMappingDto> PrepareProductAttributeMappingDtoAsync(
             ProductAttributeMapping productAttributeMapping)
        {
            ProductAttributeMappingDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
            {
                productAttributeMappingDto = new ProductAttributeMappingDto
                {
                    Id = productAttributeMapping.Id,
                    ProductAttributeId = productAttributeMapping.ProductAttributeId,
                    ProductAttributeName = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Name,
                    TextPrompt = productAttributeMapping.TextPrompt,
                    DefaultValue = productAttributeMapping.DefaultValue,
                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = productAttributeMapping.DisplayOrder,
                    IsRequired = productAttributeMapping.IsRequired,
                    //TODO: Somnath
                    //ProductAttributeValues = _productAttributeService.GetProductAttributeValueById(productAttributeMapping.Id).
                    //                                                .Select(x =>
                    //                                                            PrepareProductAttributeValueDto(x,
                    //                                                                                            productAttributeMapping
                    //                                                                                                .Product))
                    //                                                .ToList()
                };
            }

            return productAttributeMappingDto;
        }

        public async Task<CustomerDto> PrepareCustomerDTOAsync(Customer customer)
        {
            var result = customer.ToDto();
            var customerRoles = await this._customerService.GetCustomerRolesAsync(customer);
            foreach (var item in customerRoles)
            {
                result.RoleIds.Add(item.Id);
            }

            return result;
        }

        private void PrepareProductAttributeCombinations(IEnumerable<ProductAttributeCombination> productAttributeCombinations,
            ProductDto productDto)
        {
            productDto.ProductAttributeCombinations = productDto.ProductAttributeCombinations ?? new List<ProductAttributeCombinationDto>();

            foreach (var productAttributeCombination in productAttributeCombinations)
            {
                var productAttributeCombinationDto = PrepareProductAttributeCombinationDto(productAttributeCombination);
                if (productAttributeCombinationDto != null)
                {
                    productDto.ProductAttributeCombinations.Add(productAttributeCombinationDto);
                }
            }
        }

        private ProductAttributeCombinationDto PrepareProductAttributeCombinationDto(ProductAttributeCombination productAttributeCombination)
        {
            return productAttributeCombination.ToDto();
        }

        public void PrepareProductSpecificationAttributes(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
        {
            if (productDto.ProductSpecificationAttributes == null)
                productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();

            foreach (var productSpecificationAttribute in productSpecificationAttributes)
            {
                ProductSpecificationAttributeDto productSpecificationAttributeDto = PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

                if (productSpecificationAttributeDto != null)
                {
                    productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
                }
            }
        }

        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.ToDto();
        }

        public SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute)
        {
            return specificationAttribute.ToDto();
        }


        
        public TaxCategoryDto PrepareTaxCategoryDTO(TaxCategory taxCategory)
        {
            var taxRateModel = new TaxCategoryDto()
            {
                Id = taxCategory.Id,
                Name = taxCategory.Name,
                DisplayOrder = taxCategory.DisplayOrder,
                Rate = _settingService.GetSettingByKey<decimal>(string.Format(Configurations.FixedRateSettingsKey, taxCategory.Id))
            };

            return taxRateModel;
        }

        public async Task<ManufacturerDto> PrepareManufacturerDto(Manufacturer manufacturer)
        {
            var manufacturerDto = manufacturer.ToDto();

            var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
            var imageDto = PrepareImageDtoAsync(picture);

            if (imageDto != null)
            {
                manufacturerDto.Image = await imageDto;
            }

            manufacturerDto.SeName =await _urlRecordService.GetSeNameAsync(manufacturer);
            manufacturerDto.DiscountIds = (await _discountService.GetAppliedDiscountsAsync(manufacturer)).Select(discount => discount.Id).ToList();
            manufacturerDto.RoleIds = (await _aclService.GetAclRecordsAsync(manufacturer)).Select(acl => acl.CustomerRoleId).ToList();
            manufacturerDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(manufacturer)).Select(mapping => mapping.StoreId)
                .ToList();

            var allLanguages = _languageService.GetAllLanguages();

            manufacturerDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (var language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                {
                    LanguageId = language.Id,
                    LocalizedName = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name, language.Id)
                };

                manufacturerDto.LocalizedNames.Add(localizedNameDto);
            }

            return manufacturerDto;
        }

        async Task<ImageMappingDto> IDTOHelper.PrepareProductPictureDTO(ProductPicture productPicture)
        {
            return await PrepareProductImageDtoAsync(productPicture);
        }

    }
}



//protected ImageMappingDto PrepareProductImageDto(ProductPicture productPicture)
//{
//    ImageMappingDto imageMapping = null;

//    if (productPicture != null)
//    {
//        // We don't use the image from the passed dto directly 
//        // because the picture may be passed with src and the result should only include the base64 format.
//        imageMapping = new ImageMappingDto
//        {
//            //Attachment = Convert.ToBase64String(picture.PictureBinary),
//            Id = productPicture.Id,
//            ProductId = productPicture.ProductId,
//            PictureId = productPicture.PictureId,
//            Position = productPicture.DisplayOrder,
//            MimeType = productPicture.Picture.MimeType,
//            Src = _pictureService.GetPictureUrl(productPicture.Picture)
//        };
//    }

//    return imageMapping;
//}