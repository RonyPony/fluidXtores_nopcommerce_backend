using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
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
using Nop.Plugin.Misc.FluidApi.DTO.ShoppingCarts;
using Nop.Plugin.Misc.FluidApi.DTO.SpecificationAttributes;
using Nop.Plugin.Misc.FluidApi.DTO.Stores;
using Nop.Plugin.Misc.FluidApi.DTO.TaxCategory;

namespace Nop.Plugin.Misc.FluidApi.Helpers
{
    public interface IDTOHelper
    {
        Task<CustomerDto> PrepareCustomerDTOAsync(Customer customer);
        Task<ProductDto> PrepareProductDTOAsync(Product product);
        Task<CategoryDto> PrepareCategoryDTOAsync(Category category);
        Task<OrderDto> PrepareOrderDTOAsync(Order order);
        Task<ShoppingCartItemDto> PrepareShoppingCartItemDTOAsync(ShoppingCartItem shoppingCartItem);
        Task<OrderItemDto> PrepareOrderItemDTO(OrderItem orderItem);
        Task<StoreDto> PrepareStoreDTOAsync(Store store);
        Task<LanguageDto> PrepareLanguageDtoAsync(Language language);
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
        SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);
        Task<ManufacturerDto> PrepareManufacturerDto(Manufacturer manufacturer);
        TaxCategoryDto PrepareTaxCategoryDTO(TaxCategory taxCategory);
        Task<ImageMappingDto> PrepareProductPictureDTO(ProductPicture productPicture);
      
    }
}

