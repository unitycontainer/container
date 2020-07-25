using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Resolution;
namespace Unity
{
    public partial class UnityContainer
    {
        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="regex">Pattern to match names to. Only these with successful 
        /// <see cref="Regex.IsMatch(string name)"/> will be resolved</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public IAsyncEnumerable<object?> ResolveAsync(Type type, Regex regex, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }
    }
}
