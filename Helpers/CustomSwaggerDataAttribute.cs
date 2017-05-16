using System;

namespace VSTSWebApi.Helpers
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CustomSwaggerDataAttribute : Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}