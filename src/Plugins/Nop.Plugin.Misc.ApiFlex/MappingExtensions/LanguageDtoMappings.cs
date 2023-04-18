using Nop.Core.Domain.Localization;
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.DTO.Languages;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class LanguageDtoMappings
    {
        public static LanguageDto ToDto(this Language language)
        {
            return language.MapTo<Language, LanguageDto>();
        }
    }
}
