using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Misc.ApiFlex.DataStructures;
using Nop.Services.Media;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Nop.Plugin.Misc.ApiFlex.Infrastructure.Constants;

namespace Nop.Plugin.Misc.ApiFlex.Services
{
    public class PictureApiService : IPictureApiService
    {
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IPictureService _pictureService;

        public PictureApiService(IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IRepository<Vendor> vendorRepository,
            IStoreMappingService storeMappingService, IPictureService pictureService)
        {
            _productRepository = productRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _vendorRepository = vendorRepository;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
        }

        public async Task<string> GetStoreLogoAsync(int storeId)
        {
            return await _pictureService.GetDefaultPictureUrlAsync();
        }
    }
}