using System;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class AddressFactory : IFactory<Address>
    {

        async Task<Address> IFactory<Address>.InitializeAsync()
        {
            var address = new Address()
            {
                CreatedOnUtc = DateTime.UtcNow
            };
            return address;
        }
    }
}