using Nop.Plugin.Misc.FluidApi.DTO;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public interface IProductAttributeConverter
    {
        List<ProductItemAttributeDto> Parse(string attributesXml);
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
