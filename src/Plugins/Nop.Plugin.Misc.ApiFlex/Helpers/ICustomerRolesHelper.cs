using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;
namespace Nop.Plugin.Misc.ApiFlex.Helpers
{
    public interface ICustomerRolesHelper
    {
        Task<IList<CustomerRole>> GetValidCustomerRolesAsync(List<int> roleIds);
        bool IsInGuestsRole(IList<CustomerRole> customerRoles);
        bool IsInRegisteredRole(IList<CustomerRole> customerRoles);
    }
}