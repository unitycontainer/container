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
        /// the given type with the given value.
        /// </summary>
        /// <param name="typeToConstruct">Type of the dependency.</param>
        /// <param name="dependencyValue">InjectionParameterValue to use.</param>
        public DependencyOverride(Type typeToConstruct, object dependencyValue)
            : base(null, typeToConstruct, null)
        {
            Value = dependencyValue;
        }

        public DependencyOverride(string name, object dependencyValue)
            : base(null, null, name)
        {
            Value = dependencyValue;
        }

        public DependencyOverride(Type type, string name, object value)
            : base(null, type, name)
        {
            Value = value;
        }

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
        /// Construct a new <see cref="DependencyOverride{T}"/> object that will
        /// override the given dependency, and pass the given value.
        /// </summary>
        public DependencyOverride(object dependencyValue)
            : base(null, typeof(T), null, dependencyValue)
        {
        }
    }
}
