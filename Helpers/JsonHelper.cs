using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace VSTSWebApi.Helpers
{
    public class JsonHelper
    {
        public static Dictionary<string, object> DeserializeAndFlatten(JToken token)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            FillDictionaryFromJToken(dict, token, "");
            return dict;
        }

        private static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    if (!dict.ContainsKey(prefix))
                    {
                        dict.Add(prefix, ((JValue)token).Value);
                    }
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : name);
        }
    }
}