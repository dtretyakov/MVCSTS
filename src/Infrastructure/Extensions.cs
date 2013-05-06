using System.Collections.Generic;
using System.Collections.Specialized;

namespace MvcSTSApplication.Infrastructure
{
    public static class Extensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var result = new Dictionary<string, string>(collection.Count);

            foreach (string key in collection.AllKeys)
            {
                result.Add(key, collection[key]);
            }

            return result;
        }
    }
}