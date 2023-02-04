using System;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class CustomerFactory : IFactory<Customer>
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