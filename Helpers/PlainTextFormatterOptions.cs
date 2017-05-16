using System.Text;

namespace VSTSWebApi.Helpers
{
    public class PlainTextFormatterOptions
    {
        public Encoding[] SupportedEncodings { get; set; } = { Encoding.UTF8 };

        public string[] SupportedMediaTypes { get; set; } = { "text/plain" };
    }
}