using System;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.FluidApi.Factories
{
    public class AddressFactory : Factory<Address>
    {

        async Task<Address> Factory<Address>.Initialize()
        {
            var address = new Address()
            {
                CreatedOnUtc = DateTime.UtcNow
            };
            return address;
        }
    }
}