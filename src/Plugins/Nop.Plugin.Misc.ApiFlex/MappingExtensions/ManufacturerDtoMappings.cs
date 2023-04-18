using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Manufacturers;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ManufacturerDtoMappings
    {
        public static ManufacturerDto ToDto(this Manufacturer manufacturer)
        {
            return manufacturer.MapTo<Manufacturer, ManufacturerDto>();
        }
    }
}