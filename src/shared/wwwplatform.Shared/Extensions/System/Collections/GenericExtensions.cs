using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Shared.Extensions.System.Collections
{
    public static class GenericExtensions
    {
        public static List<T> RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            List<T> removed = new List<T>();
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                T element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                    removed.Add(element);
                }
            }
            return removed;
        }
    }
}
