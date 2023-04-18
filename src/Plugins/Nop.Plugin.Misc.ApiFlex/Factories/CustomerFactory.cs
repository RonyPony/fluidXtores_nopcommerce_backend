using System;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ApiFlex.Factories
{
    public class CustomerFactory : Factory<Customer>
    {
        public async Task<Customer> Initialize()
        {
            var defaultCustomer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                Active = true
            };
            return defaultCustomer;
        }
  
        
    }
}