using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.DTO.CustomerRoles;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.MappingExtensions;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    public class CustomerRolesController 
    {
        ICustomerService _customerServ;
        IJsonFieldsSerializer _jsonFieldsSerializer;
        public CustomerRolesController(IJsonFieldsSerializer jsonFieldsSerializer,
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

            IList<CustomerRoleDto> customerRolesAsDto = allCustomerRoles.Select(role => role.ToDto()).ToList();

            var customerRolesRootObject = new CustomerRolesRootObject()
            {
                CustomerRoles = customerRolesAsDto
            };

            var json = _jsonFieldsSerializer.Serialize(customerRolesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
