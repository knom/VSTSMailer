using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using VSTSWebApi.Controllers;

namespace VSTSWebApi.Helpers
{
    public class CustomSwaggerDataOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var desc = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            var extensions = desc.MethodInfo.GetCustomAttributes<CustomSwaggerDataAttribute>();
            foreach (var ex in extensions)
            {
                operation.Extensions.Add(ex.Key, ex.Value);
            }

            var parameters = context.ApiDescription.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>();
            foreach (var par in parameters)
            {
                var ext2 = par.ParameterInfo.GetCustomAttributes<CustomSwaggerDataAttribute>();
                foreach (var ex in ext2)
                {
                    operation.Parameters.Single(p => p.Name == par.Name).Extensions.Add(ex.Key, ex.Value);
                }
            }
        }
    }
}