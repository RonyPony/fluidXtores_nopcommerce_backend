using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.FluidApi.Helpers
{
    public interface ICustomerRolesHelper
    {
        Task<IList<CustomerRole>> GetValidCustomerRolesAsync(List<int> roleIds);
        bool IsInGuestsRole(IList<CustomerRole> customerRoles);
        bool IsInRegisteredRole(IList<CustomerRole> customerRoles);
    }
}