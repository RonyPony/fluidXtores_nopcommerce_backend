using System;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.FluidApi.Infrastructure
{
    public static class Constants
    {
        public static class Roles
        {
            public const string ApiRoleSystemName = "ApiUserRole";

            public const string ApiRoleName = "Api Users";
        }

        public static class ViewNames
        {
            public const string AdminLayout = "_AdminLayout";
            public const string AdminApiSettings = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Settings.cshtml";
            public const string AdminApiClientsCreateOrUpdate = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Clients/CreateOrUpdate.cshtml";
            public const string AdminApiClientsSettings = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Clients/ClientSettings.cshtml";
            public const string AdminApiClientsList = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Clients/List.cshtml";
            public const string AdminApiClientsCreate = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Clients/Create.cshtml";
            public const string AdminApiClientsEdit = "~/Plugins/Nop.Plugin.Misc.FluidApi/Views/Clients/Edit.cshtml";
        }

        public static class Configurations
        {
            public const int DefaultAccessTokenExpirationInDays = 30; // 30 days

            
            public const int DefaultLimit = 50;
            public const int DefaultPageValue = 1;
            public const int DefaultSinceId = 0;
            public const int DefaultCustomerId = 0;
            public const string DefaultOrder = "Id";
            public const int MaxLimit = Int32.MaxValue;

            public const int MinLimit = 1;


       
            public const string PublishedStatus = "published";
            public const string UnpublishedStatus = "unpublished";
            public const string AnyStatus = "any";
        

            public const string FixedRateSettingsKey = "Tax.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}";

            //public const string PublishedStatus = "published";
            //public const string UnpublishedStatus = "unpublished";
            //public const string AnyStatus = "any";
            public static CacheKey JsonTypeMapsPattern => new CacheKey("json.maps");

            public static CacheKey NEWSLETTER_SUBSCRIBERS_KEY = new CacheKey("Nop.api.newslettersubscribers");
        }
    }
}
