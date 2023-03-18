using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.FluidApi.Attributes
{
    public abstract class BaseValidationAttribute : Attribute
    {
        public abstract void Validate(object instance);
        public  abstract Dictionary<string, string> GetErrors();
    }
}