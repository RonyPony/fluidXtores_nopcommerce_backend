using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using System.Threading.Tasks;
namespace Nop.Plugin.Misc.ApiFlex.Factories
{
    public class OrderFactory : Factory<Order>
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