using Nop.Core.Domain.Cms;
using Nop.Services.Common;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ApiFlex.Infrastructure
{
    internal class apiFlexPlugin:BasePlugin
    {
        public apiFlexPlugin()
        {
                
        }

        public override async Task InstallAsync()
        {
           await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }

   
}
