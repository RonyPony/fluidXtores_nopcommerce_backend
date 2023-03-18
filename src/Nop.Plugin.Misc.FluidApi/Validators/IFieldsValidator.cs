using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Validators
{
    public interface IFieldsValidator
    {
        //TODO: Why this needs to be dictionary???
        Dictionary<string, bool> GetValidFields(string fields, Type type);
    }
}