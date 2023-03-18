﻿using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public interface IOrderItemApiService
    {
        Task<IList<OrderItem>> GetOrderItemsForOrderAsync(Order order, int limit, int page, int sinceId);
        Task<int> GetOrderItemsCountAsync(Order order);
    }
}