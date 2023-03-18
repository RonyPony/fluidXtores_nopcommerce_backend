using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;

namespace Nop.Plugin.Misc.ApiFlex.DTO.Customers
{
    public class CustomersRootObject : ISerializableObject
    {
        public CustomersRootObject()
        {
            Customers = new List<CustomerDto>();
        }

        [JsonProperty("customers")]
        public IList<CustomerDto> Customers { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "customers";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(CustomerDto);
        }
    }
}