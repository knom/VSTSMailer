using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VSTSWebApi.Helpers
{
    public class AddApiKeyOperationsFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            //var authorizeFilter = context.ApiDescription.ActionDescriptor.FilterDescriptors.Select(filterInfo => filterInfo.Filter)
            //    .OfType<AuthorizeFilter>();

            var authorizeFilter = ((ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).MethodInfo.GetCustomAttributes<SwaggerAuthorize>();

            // var allowAnonymous = ((ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).MethodInfo
            //     .GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (authorizeFilter.Any())// && !allowAnonymous)
            {
                var filter = authorizeFilter.Single();

                // var roles = authorizeFilter.Single()
                //     .Policy.Requirements.OfType<RolesAuthorizationRequirement>().Single().AllowedRoles;

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
                operation.Security.Add(new Dictionary<string, IEnumerable<string>>(){
                    {filter.SecurityDefinition, new string[0]}
                });
                // operation.Parameters?.Add(new NonBodyParameter
                // {
                //     Name = "x-auth-token",
                //     @In = "header",
                //     Description = "access token",
                //     Required = true,
                //     Type = "string"
                // });
            }
        }
    }

    //public class VstsPatAuthorizationHandler : AuthorizationHandler<VstsPatRequirement>
    //{
    //    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VstsPatRequirement requirement)
    //    {
    //        context.Succeed(requirement);
    //        return Task.FromResult(0);
    //    }
    //}

    //public class VstsPatRequirement : IAuthorizationRequirement
    //{
    //}
}