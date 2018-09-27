

using System;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride
    {
        private readonly Type _typeToConstruct;

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// the given type with the given value.
        /// </summary>
        /// <param name="typeToConstruct">Type of the dependency.</param>
        /// <param name="dependencyValue">Value to use.</param>
        public DependencyOverride(Type typeToConstruct, object dependencyValue)
            : base(null, dependencyValue)
        {
            _typeToConstruct = typeToConstruct;
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            IResolverPolicy result = null;
            if (dependencyType == _typeToConstruct)
            {
                result = Value.GetResolverPolicy(dependencyType);
            }
            return result;
        }
    }

    /// <summary>
    /// A convenience version of <see cref="DependencyOverride"/> that lets you
    /// specify the dependency type using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency to override.</typeparam>
    public class DependencyOverride<T> : DependencyOverride
    {
        /// <summary>
        /// Construct a new <see cref="DependencyOverride{T}"/> object that will
        /// override the given dependency, and pass the given value.
        /// </summary>
        public DependencyOverride(object dependencyValue)
            : base(typeof(T), dependencyValue)
        {
        }
    }
}
