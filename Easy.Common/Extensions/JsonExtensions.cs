using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    public static class JsonExtensions
    {
        public static T JsonTo<T>(this Dictionary<string, string> map, string key)
        {
            return Get<T>(map, key);
        }

        /// <summary>
        /// Get JSON string value converted to T
        /// </summary>
        public static T Get<T>(this Dictionary<string, string> map, string key)
        {
            string strVal;
            return map.TryGetValue(key, out strVal) ? JsonSerializer.DeserializeFromString<T>(strVal) : default(T);
        }

        /// <summary>
        /// Get JSON string value
        /// </summary>
        public static string Get(this Dictionary<string, string> map, string key)
        {
            string strVal;
            return map.TryGetValue(key, out strVal) ? JsonTypeSerializer.Instance.UnescapeString(strVal) : null;
        }

        public static JsonArrayObjects ArrayObjects(this string json)
        {
            return JsonArrayObjects.Parse(json);
        }

        public static List<T> ConvertAll<T>(this JsonArrayObjects jsonArrayObjects, Func<JsonObject, T> converter)
        {
            var results = new List<T>();

            foreach (var jsonObject in jsonArrayObjects)
            {
                results.Add(converter(jsonObject));
            }

            return results;
        }

        public static T ConvertTo<T>(this JsonObject jsonObject, Func<JsonObject, T> converFn)
        {
            return jsonObject == null
                ? default(T)
                : converFn(jsonObject);
        }

        public static Dictionary<string, string> ToDictionary(this JsonObject jsonObject)
        {
            return jsonObject == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(jsonObject);
        }
    }
}
