using System;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride, 
                                      IEquatable<(Type,string)>,
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
            : base(name)
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
            return (Type, Name).GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is DependencyOverride dependency)
                return (dependency.Target == Target) &&
                       (dependency.Type == Type) &&
                       (dependency.Name == Name);

            return false;
        }

        public bool Equals((Type, string) other)
        {
            return (null == Type || other.Item1 == Type) &&
                   (null == Name || other.Item2 == Name);
        }

        #endregion


        #region IResolverPolicy

        public object? Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            if (Value is IResolve policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory<Type> factory && null != Type)
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
            : base(typeof(T), dependencyValue)
        {
        }
    }
}
