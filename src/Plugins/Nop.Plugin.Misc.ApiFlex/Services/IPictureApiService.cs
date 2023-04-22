using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using static Nop.Plugin.Misc.ApiFlex.Infrastructure.Constants;


namespace Nop.Plugin.Misc.ApiFlex.Services
{
    public interface IPictureApiService
    {
        Task<String> GetStoreLogoAsync(int storeId);
    }
}