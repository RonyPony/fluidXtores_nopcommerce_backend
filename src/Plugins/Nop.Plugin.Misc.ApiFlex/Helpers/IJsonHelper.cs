using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace Nop.Plugin.Misc.ApiFlex.Helpers
{
    public interface IJsonHelper
    {
        Dictionary<string, object> GetRequestJsonDictionaryFromStreamAsync(Stream stream, bool rewindStream);
        string GetRootPropertyName<T>() where T : class, new();
    }
}