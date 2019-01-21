using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqCompatibilityExtension
    {
#if NET40 || NET45 || NET46 || NET47 || NETSTANDARD1_0

        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            return source.Concat(new[] { element });
        }

#endif
    }
}
