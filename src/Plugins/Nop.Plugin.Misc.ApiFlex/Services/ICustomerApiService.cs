using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.ApiFlex.DTO.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Nop.Plugin.Misc.ApiFlex.Infrastructure.Constants;

namespace Nop.Plugin.Misc.ApiFlex.Services
{
    public interface ICustomerApiService
    {
        int GetCustomersCount();

        Task<CustomerDto> GetCustomerByIdAsync(int id, bool showDeleted = false);

        Customer GetCustomerEntityById(int id);

        IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        IList<CustomerDto> Search(string query = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);

    }
}