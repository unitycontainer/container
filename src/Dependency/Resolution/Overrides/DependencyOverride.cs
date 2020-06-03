using System;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride, 
                                      IEquatable<NamedType>,
                                      IResolve
    {
        #region Fields

        protected readonly object Value;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type type, object value)
            : base(null, type, null)
        {
            Value = value;
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(string name, object value)
            : base(null, null, name)
        {
            Value = value;
        }


        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <param name="name">Name of the dependency</param>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type type, string name, object value)
            : base(null, type, name)
        {
            Value = value;
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependency on cpecific type matching the given type and a name
        /// </summary>
        /// <param name="target">Target type</param>
        /// <param name="name">Name of the dependency</param>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="value">Value to override with</param>
        public DependencyOverride(Type target, Type type, string name, object value)
            : base(target, type, name)
        {
            Value = value;
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case DependencyOverride dependency:
                    return (dependency.Type == Type) &&
                           (dependency.Name == Name);
                
                case NamedType type:
                    return Equals(type);

                default:
                    return false;
            }
        }

        public bool Equals(NamedType other)
        {
            return (other.Type == Type) &&
                   (other.Name == Name);
        }

        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            if (Value is IResolve policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory<Type> factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion
    }

    /// <summary>
    /// A convenience version of <see cref="DependencyOverride"/> that lets you
    /// specify the dependency type using generic syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency to override.</typeparam>
    public class DependencyOverride<T> : DependencyOverride
    {
        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type and a name
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type and name match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="name">Name of the dependency</param>
        /// <param name="value">Override value</param>
        public DependencyOverride(string name, object value)
            : base(null, typeof(T), name, value)
        {
        }

        /// <summary>
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// dependencies matching the given type
        /// </summary>
        /// <remarks>
        /// This constructor creates an override that will match with any
        /// target type as long as the dependency type match. To 
        /// target specific type use <see cref="ResolverOverride.OnType(Type)"/> 
        /// method.
        /// </remarks>
        /// <param name="value">Override value</param>
        public DependencyOverride(object value)
            : base(null, typeof(T), null, value)
        {
        }
    }
}
