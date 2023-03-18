using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.CustomerRoles;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class CustomerRoleDtoMappings
    {
        public static CustomerRoleDto ToDto(this CustomerRole customerRole)
        {
            return customerRole.MapTo<CustomerRole, CustomerRoleDto>();
        }
    }
}
