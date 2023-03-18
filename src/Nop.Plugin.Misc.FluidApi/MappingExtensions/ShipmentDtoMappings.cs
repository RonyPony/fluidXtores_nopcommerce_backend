using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.Shipments;
using Nop.Plugin.Misc.FluidApi.DTO.Stores;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ShipmentDtoMappings
    {
        public static ShipmentDto ToDto(this Shipment shipment)
        {
            return shipment.MapTo<Shipment, ShipmentDto>();
        }
    }
}
