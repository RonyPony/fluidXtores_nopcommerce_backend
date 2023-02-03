using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.FluidApi.DTO.ShoppingCarts;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
        }
    }
}