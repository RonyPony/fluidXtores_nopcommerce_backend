using System;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.FluidApi
{
	public class FluidApiSettings: ISettings
    {
        public string apiKey { get; set; } 

        public bool enabled { get; set; }
    }
}

