using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public interface ICategoryApiService
    {
        Category GetCategoryById(int categoryId);

        IList<Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId, 
            int? productId = null, bool? publishedStatus = null);

        int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null);
    }
}