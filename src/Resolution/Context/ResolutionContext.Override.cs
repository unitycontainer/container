using System;
using System.Reflection;

namespace Unity.Resolution
{
    public partial struct ResolutionContext
    {
        public ResolverOverride? GetOverride(ParameterInfo parameter)
        {
            var overrides = Overrides;

            // Process overrides if any
            if (null == overrides || 0 == overrides.Length) return null;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var resolverOverride = overrides[index];

                // Check if this parameter is overridden
                if (resolverOverride is IEquatable<ParameterInfo> comparable && 
                    comparable.Equals(parameter)) return resolverOverride;
            }

            return null;
        }
    }
}
