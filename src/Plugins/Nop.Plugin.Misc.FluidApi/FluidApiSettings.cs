using System;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.FluidApi
{
	public class FluidApiSettings: ISettings
    {
        public string storeId{get;set;}
        public string apiKey { get; set; } 

        public bool enabled { get; set; }

        public int AllowedClockSkewInMinutes { get; set; } = 5;

        public string SecurityKey { get; set; } = "NowIsTheTimeForAllGoodMenToComeToTheAideOfTheirCountry";
    }
}

