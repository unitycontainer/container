using System;
using System.Globalization;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// A <see cref="IResolve"/> that will attempt to
    /// resolve a value, and return null if it cannot rather than throwing.
    /// </summary>
    public class OptionalDependencyResolvePolicy : IResolve
    {
        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolvePolicy"/> object
        /// that will attempt to resolve the given name and type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        /// <param name="name">Name to resolve with.</param>
        public OptionalDependencyResolvePolicy(Type type, string name)
        {
            if ((type ?? throw new ArgumentNullException(nameof(type))).GetTypeInfo().IsValueType)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Constants.OptionalDependenciesMustBeReferenceTypes,
                        type.GetTypeInfo().Name));
            }

            DependencyType = type;
            Name = name;
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolvePolicy"/> object
        /// that will attempt to resolve the given type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        public OptionalDependencyResolvePolicy(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Type this resolver will resolve.
        /// </summary>
        public Type DependencyType { get; }

        /// <summary>
        /// Name this resolver will resolve.
        /// </summary>
        public string Name { get; }

        #region IResolverPolicy Members

        /// <summary>
        /// GetOrDefault the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            try
            {
                return context.Resolve(DependencyType, Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
