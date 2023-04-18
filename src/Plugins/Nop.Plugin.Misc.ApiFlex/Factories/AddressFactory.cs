using System;
using Nop.Core.Domain.Common;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ApiFlex.Factories
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