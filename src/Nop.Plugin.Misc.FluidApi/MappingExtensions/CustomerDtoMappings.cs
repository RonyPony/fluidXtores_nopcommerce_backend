using Nop.Plugin.Misc.FluidApi.AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;

namespace Nop.Plugin.Misc.FluidApi.MappingExtensions
{
    public static class CustomerDtoMappings
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this Customer customer)
        {
            return customer.MapTo<Customer, OrderCustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this CustomerDto customerDto)
        {
            return customerDto.MapTo<CustomerDto, OrderCustomerDto>();
        }

        public static CustomerForShoppingCartItemDto ToCustomerForShoppingCartItemDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerForShoppingCartItemDto>();
        }
    }
}