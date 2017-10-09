// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;

namespace Unity.Policy
{
    /// <summary>
    /// A <see cref="IDependencyResolverPolicy"/> that will attempt to
    /// resolve a value, and return null if it cannot rather than throwing.
    /// </summary>
    public class OptionalDependencyResolverPolicy : IDependencyResolverPolicy
    {
        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given name and type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        /// <param name="name">Name to resolve with.</param>
        public OptionalDependencyResolverPolicy(Type type, string name)
        {
            if ((type ?? throw new ArgumentNullException(nameof(type))).GetTypeInfo().IsValueType)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Messages.OptionalDependenciesMustBeReferenceTypes,
                        type.GetTypeInfo().Name));
            }

            DependencyType = type;
            Name = name;
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyResolverPolicy"/> object
        /// that will attempt to resolve the given type from the container.
        /// </summary>
        /// <param name="type">Type to resolve. Must be a reference type.</param>
        public OptionalDependencyResolverPolicy(Type type)
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

        #region IDependencyResolverPolicy Members

        /// <summary>
        /// Get the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve(IBuilderContext context)
        {
            var newKey = new NamedTypeBuildKey(DependencyType, Name);
            try
            {
                return (context ?? throw new ArgumentNullException(nameof(context))).NewBuildUp(newKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
