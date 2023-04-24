namespace Nop.Plugin.Misc.ApiFlex.Authorization.Policies
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Nop.Plugin.Misc.ApiFlex.Authorization.Requirements;

    public class ActiveApiPluginAuthorizationPolicy : AuthorizationHandler<ActiveApiPluginRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveApiPluginRequirement requirement)
        {
            if (requirement.IsActive())
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}