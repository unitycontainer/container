using System;
using System.Diagnostics;

namespace Unity
{
    /// <summary>
    /// An <see cref="DependencyResolutionAttribute"/> used to mark a dependency
    /// as optional - the container will try to resolve it, and return null
    /// if the resolution fails rather than throw.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OptionalDependencyAttribute : DependencyResolutionAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static OptionalDependencyAttribute Instance = new OptionalDependencyAttribute();

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object.
        /// </summary>
        public OptionalDependencyAttribute()
            : base(null) { }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object that
        /// specifies a named dependency.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalDependencyAttribute(string name)
            : base(name) { }
    }
}
