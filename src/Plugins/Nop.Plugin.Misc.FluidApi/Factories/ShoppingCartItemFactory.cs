using System;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class ShoppingCartItemFactory : Factory<ShoppingCartItem>
    {
        public async Task<ShoppingCartItem> Initialize()
        {
            var newShoppingCartItem = new ShoppingCartItem();
            newShoppingCartItem.CreatedOnUtc = DateTime.UtcNow;
            newShoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            return newShoppingCartItem;
        }
    }
}