﻿using Nop.Core.Domain.Catalog;
using System;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class ManufacturerFactory : IFactory<Manufacturer>
    {
        private readonly CatalogSettings _catalogSettings;

        public ManufacturerFactory(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }

        public async Task<Manufacturer> InitializeAsync()
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