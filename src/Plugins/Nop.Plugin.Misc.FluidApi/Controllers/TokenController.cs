using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.FluidApi.Configuration;
using Nop.Plugin.Misc.FluidApi.Domain;
using Nop.Plugin.Misc.FluidApi.Infrastructure;
using Nop.Plugin.Misc.FluidApi.Models.Authentication;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{
    [AllowAnonymous]
    public class TokenController : Controller
    {
        private readonly ApiConfiguration _apiConfiguration;
        private readonly ApiSettings _apiSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;

        public TokenController(
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings
            //ApiSettings apiSettings,
            //ApiConfiguration apiConfiguration
            )
        {
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            //_apiSettings = apiSettings;
            //_apiConfiguration = apiConfiguration;
        }

        [Route("/klk")]
        [HttpGet]
        public async Task<IActionResult> saludo()
        {
            return Ok("klk");
        }

        [Route("/token")]
        [HttpGet]
        public async Task<IActionResult> CreateAsync(TokenRequest model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                return Json(new TokenResponse("Missing username"));
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return Json(new TokenResponse("Missing password"));
            }

            var customer = await ValidateUserAsync(model);

            if (customer != null)
            {
                return Json(GenerateTokenAsync(customer));
            }

            return BadRequest(new TokenResponse("Access Denied"));// new HttpStatusCodeResult(HttpStatusCode.Forbidden);// Json(new TokenResponse("Access Denied"));
        }

        private async Task<CustomerLoginResults> LoginCustomerAsync(TokenRequest model)
        {
            var loginResult = await _customerRegistrationService
                .ValidateCustomerAsync(model.Username, model.Password);

            return loginResult;
        }

        private async Task<Customer> ValidateUserAsync(TokenRequest model)
        {
            var result = await LoginCustomerAsync(model);

            if (result == CustomerLoginResults.Successful)
            {
                var customer = _customerSettings.UsernamesEnabled
                                   ? await _customerService.GetCustomerByUsernameAsync(model.Username)
                                   : await _customerService.GetCustomerByEmailAsync(model.Username);


                //activity log
                await _customerActivityService.InsertActivityAsync(customer, "Api.TokenRequest", "User API token request", customer);

                return customer;
            }

            return null;
        }

        private int GetTokenExpiryInDays()
        {
            return _apiSettings.TokenExpiryInDays <= 0
                       ? Constants.Configurations.DefaultAccessTokenExpirationInDays
                       : _apiSettings.TokenExpiryInDays;
        }

        private async Task<TokenResponse> GenerateTokenAsync(Customer customer)
        {
            var expiresInSeconds = new DateTimeOffset(DateTime.Now.AddDays(GetTokenExpiryInDays())).ToUnixTimeSeconds();

            var claims = new List<Claim>
                         {
                             new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                             new Claim(JwtRegisteredClaimNames.Exp, expiresInSeconds.ToString()),
                             new Claim(ClaimTypes.Email, customer.Email),
                             new Claim(ClaimTypes.NameIdentifier, customer.CustomerGuid.ToString()),
                             _customerSettings.UsernamesEnabled
                                 ? new Claim(ClaimTypes.Name, customer.Username)
                                 : new Claim(ClaimTypes.Name, customer.Email),

                         };

            foreach (var customerRole in await _customerService.GetCustomerRolesAsync(customer, false))
            {
                claims.Add(new Claim(ClaimTypes.Role, customerRole.Name));
            }

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.SecurityKey)),
                                                            SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claims));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);


            return new TokenResponse(accessToken, expiresInSeconds);
        }
    }
}
