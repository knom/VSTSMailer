using VSTSWebApi.Helpers;

namespace VSTSWebApi.Models
{
    public class TemplateFormatRequest
    {
        [CustomSwaggerData(Key = "description", Value = "The Razor template to use for formatting")]
        public string Template { get; set; }

        [CustomSwaggerData(Key = "description", Value = "The model to use for parameters of the template.")]
        public object Model { get; set; }
    }
}