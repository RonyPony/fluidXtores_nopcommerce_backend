using Nop.Core.Domain.Localization;
using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Plugin.Misc.FluidApi.DTO.Languages;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class LanguageDtoMappings
    {
        public static LanguageDto ToDto(this Language language)
        {
            return language.MapTo<Language, LanguageDto>();
        }
    }
}
