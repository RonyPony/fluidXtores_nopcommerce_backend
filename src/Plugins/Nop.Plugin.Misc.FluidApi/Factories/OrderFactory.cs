using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class OrderFactory : IFactory<Order>
    {
        public async Task<Order> Initialize()
        {
            var order = new Order();
            order.CreatedOnUtc = DateTime.UtcNow;
            order.OrderGuid = Guid.NewGuid();
            order.PaymentStatus = PaymentStatus.Pending;
            order.ShippingStatus = ShippingStatus.NotYetShipped;
            order.OrderStatus = OrderStatus.Pending;

            return order;
        }
    }
}