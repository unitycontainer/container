using System;
using System.Diagnostics;

namespace Unity
{
    /// <summary>
    /// This attribute is used to mark properties and parameters as targets for injection.
    /// </summary>
    /// <remarks>
    /// For properties, this attribute is necessary for injection to happen. For parameters,
    /// it's not needed unless you want to specify additional information to control how
    /// the parameter is resolved.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
    public sealed class DependencyAttribute : DependencyResolutionAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static DependencyAttribute Instance = new DependencyAttribute();

        /// <summary>
        /// Create an instance of <see cref="DependencyAttribute"/> with no name.
        /// </summary>
        public DependencyAttribute()
            : base(null) { }

        /// <summary>
        /// Create an instance of <see cref="DependencyAttribute"/> with the given name.
        /// </summary>
        /// <param name="name">Name to use when resolving this dependency.</param>
        public DependencyAttribute(string name)
            : base(name) { }
    }
}
