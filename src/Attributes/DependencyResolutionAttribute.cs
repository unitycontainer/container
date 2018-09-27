

using System;
using Unity.Policy;

namespace Unity.Attributes
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute
    {
        /// <summary>
        /// Create an instance of <see cref="IResolverPolicy"/> that
        /// will be used to get the value for the member this attribute is
        /// applied to.
        /// </summary>
        /// <param name="typeToResolve">Type of parameter or property that
        /// this attribute is decoration.</param>
        /// <returns>The resolver object.</returns>
        public abstract IResolverPolicy CreateResolver(Type typeToResolve);
    }
}
