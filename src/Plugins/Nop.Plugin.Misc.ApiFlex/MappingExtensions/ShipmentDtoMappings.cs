using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Shipments;
using Nop.Plugin.Misc.ApiFlex.DTO.Stores;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ShipmentDtoMappings
    {
        public static ShipmentDto ToDto(this Shipment shipment)
        {
            return shipment.MapTo<Shipment, ShipmentDto>();
        }
    }
}
