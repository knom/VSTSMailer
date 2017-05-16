using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VSTSWebApi.Controllers
{
    public static class RazorHelpers
    {
        public static string GetShortUser(JToken user)
        {
            string userString = (string)user;
            return userString.Substring(0, userString.IndexOf("<"));
        }

        public static string GetUserEmail(JToken user)
        {
            string userString = (string)user;
            return Regex.Match(userString, "<(.*)>").Groups[1].Value;
        }

        public static string GetShortDateString(JToken date)
        {
            var enus = new CultureInfo("en-us");
            var d = DateTime.Parse((string)date, enus);
            return d.ToString("d", enus);
        }
    }
}