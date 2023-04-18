
using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Plugin.Misc.ApiFlex.Domain;
using Nop.Plugin.Misc.ApiFlex.Models;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class ConfigurationMappings
    {
        public static ConfigurationModel ToModel(this ApiSettings apiSettings)
        {
            return apiSettings.MapTo<ApiSettings, ConfigurationModel>();
        }

        public static ApiSettings ToEntity(this ConfigurationModel apiSettingsModel)
        {
            return apiSettingsModel.MapTo<ConfigurationModel, ApiSettings>();
        }
    }
}