using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VSTSWebApi.Helpers
{
    public class CustomSwaggerDataSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            // var properties = context.SystemType.GetTypeInfo().GetProperties();
            // foreach (var property in properties)
            // {
            //     var extensions = property.GetCustomAttributes<CustomSwaggerDataAttribute>();
            //     foreach (var ex in extensions)
            //     {
            //         model.Properties.SingleOrDefault(p=>p.Key == property.Name)?.
            //         model.Properties.Extensions.Add(ex.Key, ex.Value);
            //     }
            // }

            var extensions = context.SystemType.GetTypeInfo().GetCustomAttributes<CustomSwaggerDataAttribute>();
            foreach (var ex in extensions)
            {
                model.Extensions.Add(ex.Key, ex.Value);
            }

            //var parameters = context.ApiDescription.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>();
            //foreach (var par in parameters)
            //{
            //    var ext2 = par.ParameterInfo.GetCustomAttributes<CustomSwaggerDataAttribute>();
            //    foreach (var ex in ext2)
            //    {
            //        operation.Parameters.Single(p => p.Name == par.Name).Extensions.Add(ex.Key, ex.Value);
            //    }
            //}
        }
    }
}