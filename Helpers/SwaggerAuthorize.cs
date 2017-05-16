using System;

namespace VSTSWebApi.Helpers
{
    internal class SwaggerAuthorize : Attribute
    {
        public SwaggerAuthorize(string securityDefintion)
        {
            this.SecurityDefinition = securityDefintion;
        }
        public string SecurityDefinition { get; set; }
    }
}