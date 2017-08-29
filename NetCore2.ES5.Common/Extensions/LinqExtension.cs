using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore2.ES5.Common.Extensions
{
    public static class LinqExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }


        public static Task<List<TSource>> DistinctByAsync<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Task.Run(() => {
                List<TSource> result = new List<TSource>();
                HashSet<TKey> seenKeys = new HashSet<TKey>();
                foreach (TSource element in source)
                {
                    if (seenKeys.Add(keySelector(element)))
                    {
                        result.Add(element);
                    }
                }
                return result;
            });

        }

    }
}
