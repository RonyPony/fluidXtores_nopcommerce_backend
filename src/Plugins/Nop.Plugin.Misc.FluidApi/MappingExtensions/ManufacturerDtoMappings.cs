using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.Manufacturers;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class ManufacturerDtoMappings
    {
        public static ManufacturerDto ToDto(this Manufacturer manufacturer)
        {
            return manufacturer.MapTo<Manufacturer, ManufacturerDto>();
        }
    }
}