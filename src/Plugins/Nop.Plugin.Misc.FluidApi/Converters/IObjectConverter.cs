using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Converters
{
    public interface IObjectConverter
    {
        T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
            where T : class, new();
    }
}