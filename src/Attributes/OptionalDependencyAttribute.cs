// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Policy;

namespace Unity.Attributes
{
    /// <summary>
    /// An <see cref="DependencyResolutionAttribute"/> used to mark a dependency
    /// as optional - the container will try to resolve it, and return null
    /// if the resolution fails rather than throw.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class OptionalDependencyAttribute : DependencyResolutionAttribute
    {
        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object.
        /// </summary>
        public OptionalDependencyAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object that
        /// specifies a named dependency.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalDependencyAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the dependency.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create an instance of <see cref="IDependencyResolverPolicy"/> that
        /// will be used to get the value for the member this attribute is
        /// applied to.
        /// </summary>
        /// <param name="typeToResolve">Type of parameter or property that
        /// this attribute is decoration.</param>
        /// <returns>The resolver object.</returns>
        public override IDependencyResolverPolicy CreateResolver(Type typeToResolve)
        {
            return new OptionalDependencyResolverPolicy(typeToResolve, Name);
        }
    }
}
