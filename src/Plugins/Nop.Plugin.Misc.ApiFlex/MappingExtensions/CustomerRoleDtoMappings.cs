using Nop.Plugin.Misc.ApiFlex.AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.ApiFlex.DTO.CustomerRoles;

namespace Nop.Plugin.Misc.ApiFlex.MappingExtensions
{
    public static class CustomerRoleDtoMappings
    {
        public static CustomerRoleDto ToDto(this CustomerRole customerRole)
        {
            return customerRole.MapTo<CustomerRole, CustomerRoleDto>();
        }
    }
}
