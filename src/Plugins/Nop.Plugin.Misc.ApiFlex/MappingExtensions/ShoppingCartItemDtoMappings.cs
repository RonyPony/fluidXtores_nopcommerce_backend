using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.ApiFlex.DTO.ShoppingCarts;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
        }
    }
}