using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public interface IShoppingCartItemApiService
    {
        List<ShoppingCartItem> GetShoppingCartItems(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                                                    DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit, 
                                                    int page = Configurations.DefaultPageValue);

        ShoppingCartItem GetShoppingCartItem(int id);
    }
}