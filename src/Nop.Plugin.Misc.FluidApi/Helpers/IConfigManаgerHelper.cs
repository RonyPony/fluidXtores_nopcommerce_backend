using Nop.Data;

namespace Nop.Plugin.Misc.FluidApi.Helpers
{
    public interface IConfigManagerHelper
    {
        void AddBindingRedirects();
        void AddConnectionString();
        DataSettingsManager DataSettings { get; }
    }
}