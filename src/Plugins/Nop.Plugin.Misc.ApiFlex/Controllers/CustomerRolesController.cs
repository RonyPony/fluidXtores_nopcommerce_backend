using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.ApiFlex.Attributes;
using Nop.Plugin.Misc.ApiFlex.DTO.CustomerRoles;
using Nop.Plugin.Misc.ApiFlex.DTO.Errors;
using Nop.Plugin.Misc.ApiFlex.JSON.ActionResults;
using Nop.Plugin.Misc.ApiFlex.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ApiFlex.Controllers
{
    public class CustomerRolesController 
    {
        ICustomerService _customerServ;
        IJsonFieldsSerializer _jsonFieldsSerializer;
        public CustomerRolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
                                       ICustomerService customerService) 
           
        {
            ArgumentNullException.ThrowIfNull(customerService, nameof(customerService));
            ArgumentNullException.ThrowIfNull(jsonFieldsSerializer, nameof(jsonFieldsSerializer));

            _customerServ = customerService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        /// <summary>
        /// Retrieve all customer roles
        /// </summary>
        /// <param name="fields">Fields from the customer role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customer_roles")]
        [ProducesResponseType(typeof(CustomerRolesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetAllCustomerRolesAsync(string fields = "")
        {
            var allCustomerRoles = await _customerServ.GetAllCustomerRolesAsync();
            IList<CustomerRoleDto> customerRolesAsDto = new List<CustomerRoleDto>();
            foreach (var item in allCustomerRoles)
            {
                CustomerRoleDto newRole = new CustomerRoleDto();
                newRole.Id = item.Id;
                newRole.Name = item.Name;
                newRole.FreeShipping= item.FreeShipping;
                newRole.TaxExempt= item.TaxExempt;
                newRole.Active= item.Active;
                newRole.EnablePasswordLifetime= item.EnablePasswordLifetime;
                newRole.PurchasedWithProductId= item.PurchasedWithProductId;
                newRole.IsSystemRole = item.IsSystemRole;
                newRole.SystemName = item.SystemName;
                customerRolesAsDto.Add(newRole);
            }

            var customerRolesRootObject = new CustomerRolesRootObject()
            {
                CustomerRoles = customerRolesAsDto
            };

            var json = _jsonFieldsSerializer.Serialize(customerRolesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
