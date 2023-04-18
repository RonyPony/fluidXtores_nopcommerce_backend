using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.OrderItems;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class OrderItemDtoMappings
    {
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return orderItem.MapTo<OrderItem, OrderItemDto>();
        }
    }
}