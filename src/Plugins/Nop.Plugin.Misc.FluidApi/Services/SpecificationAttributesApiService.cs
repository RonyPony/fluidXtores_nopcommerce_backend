using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.FluidApi.DataStructures;
using System.Collections.Generic;
using System.Linq;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Services
{
    public class SpecificationAttributesApiService : ISpecificationAttributeApiService
    {
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributesRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributesRepository;

        public SpecificationAttributesApiService(IRepository<ProductSpecificationAttribute> productSpecificationAttributesRepository, IRepository<SpecificationAttribute> specificationAttributesRepository)
        {
            _productSpecificationAttributesRepository = productSpecificationAttributesRepository;
            _specificationAttributesRepository = specificationAttributesRepository;
        }

        public IList<ProductSpecificationAttribute> GetProductSpecificationAttributes(int? productId = null, int? specificationAttributeOptionId = null, bool? allowFiltering = null, bool? showOnProductPage = null, int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = _productSpecificationAttributesRepository.Table;

            if (productId > 0)
            {
                query = query.Where(psa => psa.ProductId == productId);
            }

            if (specificationAttributeOptionId > 0)
            {
                query = query.Where(psa => psa.SpecificationAttributeOptionId == specificationAttributeOptionId);
            }

            if (allowFiltering.HasValue)
            {
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            }

            if (showOnProductPage.HasValue)
            {
                query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
            }

            if (sinceId > 0)
            {
                query = query.Where(productAttribute => productAttribute.Id > sinceId);
            }

            query = query.OrderBy(x => x.Id);

            return new ApiList<ProductSpecificationAttribute>(query, page - 1, limit);
        }

        public IList<SpecificationAttribute> GetSpecificationAttributesAsync(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = _specificationAttributesRepository.Table;

            if (sinceId > 0)
            {
                query = query.Where(x => x.Id > sinceId);
            }

            query = query.OrderBy(x => x.Id);

            return new ApiList<SpecificationAttribute>(query, page - 1, limit);
        }
    }
}