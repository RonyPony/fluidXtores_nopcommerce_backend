using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.ApiFlex.Authorization.Requirements;

namespace Nop.Plugin.Misc.ApiFlex.Authorization.Policies
{
    public class CustomerRoleAuthorizationPolicy : AuthorizationHandler<CustomerRoleRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerRoleAuthorizationPolicy(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, CustomerRoleRequirement requirement)
        {
            if (await requirement.IsCustomerInRoleAsync())
            {
                context.Succeed(requirement);
            }
            else
            {
                var message = Encoding.UTF8.GetBytes("User authenticated but not in Api Role.");
                await this._httpContextAccessor.HttpContext.Response.Body.WriteAsync(message);
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
