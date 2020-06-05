using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Resolution
{
    /// <summary>
    /// A convenience form of <see cref="ParameterOverride"/> that lets you
    /// specify multiple parameter overrides in one shot rather than having
    /// to construct multiple objects.
    /// </summary>
    public class ParameterOverrides : IEnumerable
    {
        IList<Tuple<string, object>> _values = new List<Tuple<string, object>>();

        /// <summary>
        /// Add a new override to the collection with the given key and value.
        /// </summary>
        /// <param name="key">Key - for example, a parameter or property name.</param>
        /// <param name="value">InjectionParameterValue - the value to be returned by the override.</param>
        public void Add(string key, object value)
        {
            _values.Add(new Tuple<string, object>(key, value));
        }

        #region Type Based Override

        /// <summary>
        /// Wrap this resolver in one that verifies the type of the object being built.
        /// This allows you to narrow any override down to a specific type easily.
        /// </summary>
        /// <typeparam name="T">Type to constrain the override to.</typeparam>
        /// <returns>The new override.</returns>
        public ResolverOverride[] OnType<T>()
        {
            return _values.Select(p => new ParameterOverride(p.Item1, p.Item2).OnType<T>())
                          .ToArray();
        }

        /// <summary>
        /// Wrap this resolver in one that verifies the type of the object being built.
        /// This allows you to narrow any override down to a specific type easily.
        /// </summary>
        /// <param name="targetType">Type to constrain the override to.</param>
        /// <returns>The new override.</returns>
        public ResolverOverride[] OnType(Type targetType)
        {
            return _values.Select(p => new ParameterOverride(p.Item1, p.Item2).OnType(targetType))
                          .ToArray();
        }

        #endregion

        public IEnumerator GetEnumerator()
        {
            foreach (var tuple in _values)
            {
                yield return new ParameterOverride(tuple.Item1, tuple.Item2);
            }
        }
    }
}
