using System;

namespace Nop.Plugin.Misc.FluidApi.DTO
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}