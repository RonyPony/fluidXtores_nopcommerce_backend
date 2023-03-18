using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Maps
{
    public interface IJsonPropertyMapper
    {
        Dictionary<string, Tuple<string, Type>> GetMap(Type type);
    }
}