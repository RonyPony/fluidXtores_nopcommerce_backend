using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Orders;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class OrderDtoMappings
    {
        public static OrderDto ToDto(this Order order)
        {
            return order.MapTo<Order, OrderDto>();
        }
    }
}