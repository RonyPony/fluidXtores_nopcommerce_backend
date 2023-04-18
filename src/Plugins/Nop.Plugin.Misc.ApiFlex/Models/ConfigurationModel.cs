using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ApiFlex.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Api.Admin.EnableApi")]
        public bool EnableApi { get; set; }
        public bool EnableApi_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Api.Admin.AllowRequestsFromSwagger")]
        public bool AllowRequestsFromSwagger { get; set; }
        public bool AllowRequestsFromSwagger_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Api.Admin.EnableLogging")]
        public bool EnableLogging { get; set; }
        public bool EnableLogging_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }

    }
}