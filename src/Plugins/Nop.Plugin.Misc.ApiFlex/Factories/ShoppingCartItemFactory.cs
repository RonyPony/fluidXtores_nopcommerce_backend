using System;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;
namespace Nop.Plugin.Misc.ApiFlex.Factories
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