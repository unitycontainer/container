using System;
using System.Diagnostics;
using Unity.Exceptions;
using Unity.Resolution;

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
        #region Singleton

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static OptionalDependencyAttribute Instance = new OptionalDependencyAttribute();

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object.
        /// </summary>
        public OptionalDependencyAttribute()
            : base() { }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object that
        /// specifies a named dependency.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalDependencyAttribute(string name)
            : base(name) 
        {
            AllowDefault = true;
        }


        public OptionalDependencyAttribute(Type type)
            : base(type)
        {
            AllowDefault = true;
        }

        public OptionalDependencyAttribute(Type type, string name)
            : base(type, name)
        {
            AllowDefault = true;
        }

        #endregion


        #region Overrides

        /// <inheritdoc />
        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type) => 
            (ref TContext context) =>
            {
                try
                {
                    return context.Resolve(type, Name);
                }
                catch (Exception ex) when (!(ex.InnerException is CircularDependencyException))
                {
                    return null;
                }
            };

        #endregion
    }
}
