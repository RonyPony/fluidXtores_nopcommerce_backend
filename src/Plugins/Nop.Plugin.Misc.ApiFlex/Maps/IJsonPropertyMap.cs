using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ApiFlex.Maps
{
    public interface IJsonPropertyMapper
    {
        Dictionary<string, Tuple<string, Type>> GetMap(Type type);
    }
}