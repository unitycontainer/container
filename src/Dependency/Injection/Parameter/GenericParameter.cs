using System;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="InjectionParameterValue"/> that lets you specify that
    /// an instance of a generic type parameter should be resolved.
    /// </summary>
    public class GenericParameter : GenericParameterBase
    {
        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        public GenericParameter(string genericParameterName)
            : base(genericParameterName)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="resolutionKey">name to use when looking up in the container.</param>
        public GenericParameter(string genericParameterName, string resolutionKey)
            : base(genericParameterName, resolutionKey)
        { }

        protected override ResolveDelegate<TContext> GetResolver<TContext>(Type type, string resolutionKey)
        {
            return (ref TContext context) => context.Resolve(type, resolutionKey);
        }
    }
}
