using Nop.Core.Domain.Common;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class AddressDtoMappings
    {
        public static AddressDto ToDto(this Address address)
        {
            return address.MapTo<Address, AddressDto>();
        }

        public static Address ToEntity(this AddressDto addressDto)
        {
            return addressDto.MapTo<AddressDto, Address>();
        }
    }
}