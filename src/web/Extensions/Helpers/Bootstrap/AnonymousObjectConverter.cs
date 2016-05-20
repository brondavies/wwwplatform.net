using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace wwwplatform.Extensions.Helpers.Bootstrap
{
    public class AnonymousObjectConverter
    {
        public static IEnumerable<T> Convert<T>(IEnumerable<object> anonymousObjects) where T : Dictionary<string, object>, new()
        {
            if (anonymousObjects != null)
            {
                foreach (var anon in anonymousObjects)
                {
                    if (anon != null)
                    {
                        T dictionary = new T();
                        var type = anon.GetType();
                        IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                               .Where(prop => prop.GetIndexParameters().Length == 0 &&
                                                                              prop.GetMethod != null);

                        foreach (var property in properties)
                        {
                            dictionary[property.Name] = (int)property.GetValue(anon);
                        }
                        yield return dictionary;
                    }
                }
            }
        }
    }
}
