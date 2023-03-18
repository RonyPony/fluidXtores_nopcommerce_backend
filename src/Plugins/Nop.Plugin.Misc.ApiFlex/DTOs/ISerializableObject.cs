using System;

namespace Nop.Plugin.Misc.ApiFlex.DTO
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}