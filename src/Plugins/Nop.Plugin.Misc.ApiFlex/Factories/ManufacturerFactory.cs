using Nop.Core.Domain.Catalog;
using System;
using System.Threading.Tasks;
namespace Nop.Plugin.Misc.ApiFlex.Factories
{
    public class ManufacturerFactory : Factory<Manufacturer>
    {
        private readonly CatalogSettings _catalogSettings;

        public ManufacturerFactory(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }

        public async Task<Manufacturer> Initialize()
        {
            // TODO: cache the default entity.
            var defaultManufacturer = new Manufacturer();

            //default values
            defaultManufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
            defaultManufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
            defaultManufacturer.Published = true;
            defaultManufacturer.AllowCustomersToSelectPageSize = true;

            defaultManufacturer.CreatedOnUtc = DateTime.UtcNow;
            defaultManufacturer.UpdatedOnUtc = DateTime.UtcNow;

            return defaultManufacturer;
        }
    }
}