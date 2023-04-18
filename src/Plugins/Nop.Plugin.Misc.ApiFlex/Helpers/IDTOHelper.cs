using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Misc.ApiFlex.DTO.Categories;
using Nop.Plugin.Misc.ApiFlex.DTO.Customers;
using Nop.Plugin.Misc.ApiFlex.DTO.Images;
using Nop.Plugin.Misc.ApiFlex.DTO.Languages;
using Nop.Plugin.Misc.ApiFlex.DTO.Manufacturers;
using Nop.Plugin.Misc.ApiFlex.DTO.OrderItems;
using Nop.Plugin.Misc.ApiFlex.DTO.Orders;
using Nop.Plugin.Misc.ApiFlex.DTO.ProductAttributes;
using Nop.Plugin.Misc.ApiFlex.DTO.Products;
using Nop.Plugin.Misc.ApiFlex.DTO.ShoppingCarts;
using Nop.Plugin.Misc.ApiFlex.DTO.SpecificationAttributes;
using Nop.Plugin.Misc.ApiFlex.DTO.Stores;
using Nop.Plugin.Misc.ApiFlex.DTO.TaxCategory;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ApiFlex.Helpers
{
    public interface IDTOHelper
    {
        Task<CustomerDto> PrepareCustomerDTOAsync(Customer customer);
        Task<ProductDto> PrepareProductDTOAsync(Product product);
        Task<CategoryDto> PrepareCategoryDTOAsync(Category category);
        Task<OrderDto> PrepareOrderDTOAsync(Order order);
        Task<ShoppingCartItemDto> PrepareShoppingCartItemDTOAsync(ShoppingCartItem shoppingCartItem);
        Task<StoreDto> PrepareStoreDTOAsync(Store store);
        Task<LanguageDto> PrepareLanguageDtoAsync(Language language);
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
        SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);
        Task<ManufacturerDto> PrepareManufacturerDto(Manufacturer manufacturer);
        TaxCategoryDto PrepareTaxCategoryDTO(TaxCategory taxCategory);
        Task<ImageMappingDto> PrepareProductPictureDTO(ProductPicture productPicture);
        Task<OrderItemDto> PrepareOrderItemDTOAsync(OrderItem item);
    }
}

