using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO;
using Nop.Plugin.Misc.FluidApi.DTO.Customers;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.Factories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.MappingExtensions;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.CustomersParameters;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Linq;
using System.Net;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    //[AllowAnonymous]
    [ApiController]
    public class CustomersController:BaseApiController
    {
        #region Fields
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerRolesHelper _customerRolesHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICountryService _countryService;
        private readonly IMappingHelper _mappingHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILanguageService _languageService;
        //private readonly Factory<Customer> _factory;
        private readonly IDTOHelper _dtoHelper;
        #endregion

        // We resolve the customer settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private CustomerSettings _customerSettings;

        private CustomerSettings CustomerSettings
        {
            get
            {
                if (_customerSettings == null)
                {
                    _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
                }

                return _customerSettings;
            }
        }
        #region Ctor
        public CustomersController(
            ICustomerApiService customerApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerRolesHelper customerRolesHelper,
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            //Factory<Customer> factory,
            ICountryService countryService,
            IMappingHelper mappingHelper,
            IPluginService pluginService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            ILanguageService languageService,
            IDTOHelper dtoHelper
            ) : base(
                  jsonFieldsSerializer,
                  aclService,
                  customerService,
                  storeMappingService,
                  storeService,
                  discountService,
                  customerActivityService,
                  localizationService,
                  pictureService)



        {
            _customerApiService = customerApiService;
            //_factory = factory;
            //_countryService = countryService;
            //_mappingHelper = mappingHelper;
            //_newsLetterSubscriptionService = newsLetterSubscriptionService;
            //_languageService = languageService;
            //_encryptionService = encryptionService;
            //_genericAttributeService = genericAttributeService;
            //_customerRolesHelper = customerRolesHelper;
            //_dtoHelper = dtoHelper;
        }
        #endregion
        /// <summary>
        /// Retrieve all customers of a shop
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>    
        [HttpGet]
        [Route("/api/customers")]
        [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [AllowAnonymous]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetCustomersAsync(CustomersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                //return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            try
            {
                var createAtMinField = parameters.CreatedAtMin;
                var CreatedAtMaxField = parameters.CreatedAtMax;
                var limitField = parameters.Limit;
                var pageField = parameters.Page;
                var sinceField = parameters.SinceId;
                var allCustomers = _customerApiService.GetCustomersDtos(createAtMinField,
                CreatedAtMaxField,
                limitField,
                pageField,
                sinceField);

                var customersRootObject = new CustomersRootObject()
                {
                    Customers = allCustomers
                };

                var json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

                return new RawJsonActionResult(json);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //    /// <summary>
        //    /// Retrieve customer by spcified id
        //    /// </summary>
        //    /// <param name="id">Id of the customer</param>
        //    /// <param name="fields">Fields from the customer you want your json to contain</param>
        //    /// <response code="200">OK</response>
        //    /// <response code="404">Not Found</response>
        //    /// <response code="401">Unauthorized</response>
        //    [HttpGet]
        //    [Route("/api/customers/{id}")]
        //    [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    [GetRequestsErrorInterceptorActionFilter]
        //    public async Task<IActionResult> GetCustomerByIdAsync(int id, string fields = "")
        //    {
        //        if (id <= 0)
        //        {
        //            return Error(HttpStatusCode.BadRequest, "id", "invalid id");
        //        }

        //        var customer = await _customerApiService.GetCustomerByIdAsync(id);

        //        if (customer == null)
        //        {
        //            return Error(HttpStatusCode.NotFound, "customer", "not found");
        //        }

        //        var customersRootObject = new CustomersRootObject();
        //        customersRootObject.Customers.Add(customer);

        //        var json = JsonFieldsSerializer.Serialize(customersRootObject, fields);

        //        return new RawJsonActionResult(json);
        //    }


        //    /// <summary>
        //    /// Get a count of all customers
        //    /// </summary>
        //    /// <response code="200">OK</response>
        //    /// <response code="401">Unauthorized</response>
        //    [HttpGet]
        //    [Route("/api/customers/count")]
        //    [ProducesResponseType(typeof(CustomersCountRootObject), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //    public IActionResult GetCustomersCount()
        //    {
        //        var allCustomersCount = _customerApiService.GetCustomersCount();

        //        var customersCountRootObject = new CustomersCountRootObject()
        //        {
        //            Count = allCustomersCount
        //        };

        //        return Ok(customersCountRootObject);
        //    }

        //    /// <summary>
        //    /// Search for customers matching supplied query
        //    /// </summary>
        //    /// <response code="200">OK</response>
        //    /// <response code="400">Bad Request</response>
        //    /// <response code="401">Unauthorized</response>
        //    [HttpGet]
        //    [Route("/api/customers/search")]
        //    [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    public IActionResult Search(CustomersSearchParametersModel parameters)
        //    {
        //        if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
        //        {
        //            return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
        //        }

        //        if (parameters.Page <= 0)
        //        {
        //            return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
        //        }

        //        var customersDto = _customerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

        //        var customersRootObject = new CustomersRootObject()
        //        {

        //            Customers = customersDto
        //        };

        //        var json = JsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

        //        return new RawJsonActionResult(json);
        //    }

        //    [HttpPost]
        //    [Route("/api/customers")]
        //    [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    public async Task<IActionResult> CreateCustomerAsync([ModelBinder(typeof(JsonModelBinder<CustomerDto>))] Delta<CustomerDto> customerDelta)
        //    {
        //        // Here we display the errors if the validation has failed at some point.
        //        if (!ModelState.IsValid)
        //        {
        //            return Error();
        //        }

        //        var customer = await CustomerService.GetCustomerByEmailAsync(customerDelta.Dto.Email);
        //        if (customer != null && !customer.Deleted)
        //            return Error(HttpStatusCode.Conflict, nameof(Customer.Email), "Email is already registered");

        //        //If the validation has passed the customerDelta object won't be null for sure so we don't need to check for this.

        //        // Inserting the new customer
        //        var newCustomer = await _factory.Initialize();
        //        customerDelta.Merge(newCustomer);

        //        foreach (var address in customerDelta.Dto.Addresses)
        //        {
        //            // we need to explicitly set the date as if it is not specified
        //            // it will default to 01/01/0001 which is not supported by SQL Server and throws and exception
        //            if (address.CreatedOnUtc == null)
        //            {
        //                address.CreatedOnUtc = DateTime.UtcNow;
        //            }

        //            await CustomerService.InsertCustomerAddressAsync(newCustomer, address.ToEntity());
        //        }

        //        await CustomerService.InsertCustomerAsync(newCustomer);

        //        await InsertFirstAndLastNameGenericAttributesAsync(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, newCustomer);

        //        if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId, out var languageId)
        //                                                                && await _languageService.GetLanguageByIdAsync(languageId) != null)
        //        {
        //            await _genericAttributeService.SaveAttributeAsync(newCustomer, "LanguageId", languageId);
        //        }

        //        //password
        //        if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
        //        {
        //            AddPassword(customerDelta.Dto.Password, newCustomer);
        //        }

        //        // We need to insert the entity first so we can have its id in order to map it to anything.
        //        // TODO: Localization
        //        // TODO: move this before inserting the customer.
        //        if (customerDelta.Dto.RoleIds.Count > 0)
        //        {
        //            await AddValidRolesAsync(customerDelta, newCustomer);
        //        }

        //        // Preparing the result dto of the new customer
        //        // We do not prepare the shopping cart items because we have a separate endpoint for them.
        //        var newCustomerDto = await _dtoHelper.PrepareCustomerDTOAsync(newCustomer);

        //        // This is needed because the entity framework won't populate the navigation properties automatically
        //        // and the country will be left null. So we do it by hand here.
        //        PopulateAddressCountryNames(newCustomerDto);

        //        // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
        //        newCustomerDto.FirstName = customerDelta.Dto.FirstName;
        //        newCustomerDto.LastName = customerDelta.Dto.LastName;

        //        newCustomerDto.LanguageId = customerDelta.Dto.LanguageId;

        //        //activity log
        //        await CustomerActivityService.InsertActivityAsync("AddNewCustomer", await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomer"), newCustomer);

        //        var customersRootObject = new CustomersRootObject();

        //        customersRootObject.Customers.Add(newCustomerDto);

        //        var json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

        //        return new RawJsonActionResult(json);
        //    }

        //    [HttpPut]
        //    [Route("/api/customers/{id}")]
        //    [ProducesResponseType(typeof(CustomersRootObject), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    public async Task<IActionResult> UpdateCustomerAsync([ModelBinder(typeof(JsonModelBinder<CustomerDto>))] Delta<CustomerDto> customerDelta)
        //    {
        //        // Here we display the errors if the validation has failed at some point.
        //        if (!ModelState.IsValid)
        //        {
        //            return Error();
        //        }

        //        // Updateting the customer
        //        var currentCustomer = _customerApiService.GetCustomerEntityById(customerDelta.Dto.Id);

        //        if (currentCustomer == null)
        //        {
        //            return Error(HttpStatusCode.NotFound, "customer", "not found");
        //        }

        //        customerDelta.Merge(currentCustomer);

        //        if (customerDelta.Dto.RoleIds.Count > 0)
        //        {
        //            await AddValidRolesAsync(customerDelta, currentCustomer);
        //        }

        //        if (customerDelta.Dto.Addresses.Count > 0)
        //        {
        //            var currentCustomerAddresses = (await CustomerService.GetAddressesByCustomerIdAsync(currentCustomer.Id)).ToDictionary(address => address.Id, address => address);

        //            foreach (var passedAddress in customerDelta.Dto.Addresses)
        //            {
        //                var addressEntity = passedAddress.ToEntity();

        //                if (currentCustomerAddresses.ContainsKey(passedAddress.Id))
        //                {
        //                    _mappingHelper.Merge(passedAddress, currentCustomerAddresses[passedAddress.Id]);
        //                }
        //                else
        //                {
        //                    await CustomerService.InsertCustomerAddressAsync(currentCustomer, addressEntity);
        //                }
        //            }
        //        }

        //        await CustomerService.UpdateCustomerAsync(currentCustomer);

        //        await InsertFirstAndLastNameGenericAttributesAsync(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, currentCustomer);


        //        if (!string.IsNullOrEmpty(customerDelta.Dto.LanguageId) && int.TryParse(customerDelta.Dto.LanguageId, out var languageId)
        //                                                                && await _languageService.GetLanguageByIdAsync(languageId) != null)
        //        {
        //            await _genericAttributeService.SaveAttributeAsync(currentCustomer, "LanguageId", languageId);
        //        }

        //        //password
        //        if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
        //        {
        //            AddPassword(customerDelta.Dto.Password, currentCustomer);
        //        }

        //        // TODO: Localization

        //        // Preparing the result dto of the new customer
        //        // We do not prepare the shopping cart items because we have a separate endpoint for them.
        //        var updatedCustomer = await _dtoHelper.PrepareCustomerDTOAsync(currentCustomer);

        //        // This is needed because the entity framework won't populate the navigation properties automatically
        //        // and the country name will be left empty because the mapping depends on the navigation property
        //        // so we do it by hand here.
        //        PopulateAddressCountryNames(updatedCustomer);

        //        // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
        //        var firstNameGenericAttribute = (await _genericAttributeService.GetAttributesForEntityAsync(currentCustomer.Id, typeof(Customer).Name))
        //                                                                .FirstOrDefault(x => x.Key == "FirstName");

        //        if (firstNameGenericAttribute != null)
        //        {
        //            updatedCustomer.FirstName = firstNameGenericAttribute.Value;
        //        }

        //        var lastNameGenericAttribute = (await _genericAttributeService.GetAttributesForEntityAsync(currentCustomer.Id, typeof(Customer).Name))
        //                                                               .FirstOrDefault(x => x.Key == "LastName");

        //        if (lastNameGenericAttribute != null)
        //        {
        //            updatedCustomer.LastName = lastNameGenericAttribute.Value;
        //        }

        //        var languageIdGenericAttribute = (await _genericAttributeService.GetAttributesForEntityAsync(currentCustomer.Id, typeof(Customer).Name))
        //                                                                 .FirstOrDefault(x => x.Key == "LanguageId");

        //        if (languageIdGenericAttribute != null)
        //        {
        //            updatedCustomer.LanguageId = languageIdGenericAttribute.Value;
        //        }

        //        //activity log
        //        await CustomerActivityService.InsertActivityAsync("UpdateCustomer", await LocalizationService.GetResourceAsync("ActivityLog.UpdateCustomer"), currentCustomer);

        //        var customersRootObject = new CustomersRootObject();

        //        customersRootObject.Customers.Add(updatedCustomer);

        //        var json = JsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

        //        return new RawJsonActionResult(json);
        //    }

        //    [HttpDelete]
        //    [Route("/api/customers/{id}")]
        //    [GetRequestsErrorInterceptorActionFilter]
        //    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        //    [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        //    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        //    public async Task<IActionResult> DeleteCustomerAsync(int id)
        //    {
        //        if (id <= 0)
        //        {
        //            return Error(HttpStatusCode.BadRequest, "id", "invalid id");
        //        }

        //        var customer = _customerApiService.GetCustomerEntityById(id);

        //        if (customer == null)
        //        {
        //            return Error(HttpStatusCode.NotFound, "customer", "not found");
        //        }

        //        await CustomerService.DeleteCustomerAsync(customer);

        //        //remove newsletter subscription (if exists)
        //        foreach (var store in StoreService.GetAllStores())
        //        {
        //            var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
        //            if (subscription != null)
        //                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
        //        }

        //        //activity log
        //        await CustomerActivityService.InsertActivityAsync("DeleteCustomer",await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomer"), customer);

        //        return new RawJsonActionResult("{}");
        //    }

        //    private async Task InsertFirstAndLastNameGenericAttributesAsync(string firstName, string lastName, Customer newCustomer)
        //    {
        //        // we assume that if the first name is not sent then it will be null and in this case we don't want to update it
        //        if (firstName != null)
        //        {
        //            await _genericAttributeService.SaveAttributeAsync(newCustomer, "firstName", firstName);
        //        }

        //        if (lastName != null)
        //        {
        //            await _genericAttributeService.SaveAttributeAsync(newCustomer, "lastName", lastName);
        //        }
        //    }

        //    private async Task AddValidRolesAsync(Delta<CustomerDto> customerDelta, Customer currentCustomer)
        //    {
        //        var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
        //        foreach (var customerRole in allCustomerRoles)
        //        {
        //            if (customerDelta.Dto.RoleIds.Contains(customerRole.Id))
        //            {
        //                //new role
        //                if (!await CustomerService.IsInCustomerRoleAsync(currentCustomer, customerRole.SystemName))
        //                {
        //                    await CustomerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping()
        //                    {
        //                        CustomerId = currentCustomer.Id,
        //                        CustomerRoleId = customerRole.Id
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                if ( await CustomerService.IsInCustomerRoleAsync(currentCustomer, customerRole.SystemName))
        //                {
        //                    await CustomerService.RemoveCustomerRoleMappingAsync(currentCustomer, customerRole);
        //                }
        //            }
        //        }
        //    }

        //    private void PopulateAddressCountryNames(CustomerDto newCustomerDto)
        //    {
        //        foreach (var address in newCustomerDto.Addresses)
        //        {
        //            SetCountryNameAsync(address);
        //        }

        //        if (newCustomerDto.BillingAddress != null)
        //        {
        //            SetCountryNameAsync(newCustomerDto.BillingAddress);
        //        }

        //        if (newCustomerDto.ShippingAddress != null)
        //        {
        //            SetCountryNameAsync(newCustomerDto.ShippingAddress);
        //        }
        //    }

        //    private async Task SetCountryNameAsync(AddressDto address)
        //    {
        //        if (string.IsNullOrEmpty(address.CountryName) && address.CountryId.HasValue)
        //        {
        //            var country = await _countryService.GetCountryByIdAsync(address.CountryId.Value);
        //            address.CountryName = country.Name;
        //        }
        //    }

        //    private void AddPassword(string newPassword, Customer customer)
        //    {
        //        // TODO: call this method before inserting the customer.
        //        var customerPassword = new CustomerPassword
        //        {
        //            CustomerId = customer.Id,
        //            PasswordFormat = CustomerSettings.DefaultPasswordFormat,
        //            CreatedOnUtc = DateTime.UtcNow
        //        };

        //        switch (CustomerSettings.DefaultPasswordFormat)
        //        {
        //            case PasswordFormat.Clear:
        //                {
        //                    customerPassword.Password = newPassword;
        //                }
        //                break;
        //            case PasswordFormat.Encrypted:
        //                {
        //                    customerPassword.Password = _encryptionService.EncryptText(newPassword);
        //                }
        //                break;
        //            case PasswordFormat.Hashed:
        //                {
        //                    var saltKey = _encryptionService.CreateSaltKey(5);
        //                    customerPassword.PasswordSalt = saltKey;
        //                    customerPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, CustomerSettings.HashedPasswordFormat);
        //                }
        //                break;
        //        }

        //        CustomerService.InsertCustomerPasswordAsync(customerPassword);

        //        CustomerService.UpdateCustomerAsync(customer);
        //    }
    }
}