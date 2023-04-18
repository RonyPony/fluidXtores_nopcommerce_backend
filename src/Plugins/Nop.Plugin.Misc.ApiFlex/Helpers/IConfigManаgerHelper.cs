using Nop.Data;

namespace Nop.Plugin.Misc.ApiFlex.Helpers
{
    public interface IConfigManagerHelper
    {
        void AddBindingRedirects();
        void AddConnectionString();
        DataSettingsManager DataSettings { get; }
    }
}