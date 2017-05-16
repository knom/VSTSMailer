using Swashbuckle.AspNetCore.SwaggerGen;
using VSTSWebApi.Helpers;

namespace VSTSWebApi.Models
{
    [CustomSwaggerData(Key = "x-ms-summary", Value = "A user info object")]
    public class UserInfo
    {
        [CustomSwaggerData(Key = "description", Value = "The firstname & lastname of the user as one string.")]
        public string Name { get; set; }

        [CustomSwaggerData(Key = "description", Value = "The email address of the user.")]
        public string EMail { get; set; }
    }
}