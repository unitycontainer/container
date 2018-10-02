using System;
using Unity.Delegates;
using Unity.Policy;
using Unity.ResolverPolicy;

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

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> instance that will
        /// return this types value for the parameter.
        /// </summary>
        /// <param name="typeToResolve">The actual type to resolve.</param>
        /// <param name="resolutionKey">The resolution key.</param>
        /// <returns>The <see cref="IResolverPolicy"/>.</returns>
        protected override IResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey)
        {
            return new NamedTypeDependencyResolverPolicy(typeToResolve, resolutionKey);
        }

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
