using System;
using System.Reflection;
using Unity.Build;
using Unity.Factory;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// the value injected whenever there is a dependency of the
    /// given type, regardless of where it appears in the object graph.
    /// </summary>
    public class DependencyOverride : ResolverOverride, 
                                      IEquatable<ParameterInfo>,
                                      IEquatable<PropertyInfo>,
                                      IResolverPolicy
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

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PropertyInfo property:
                    return Equals(property);

                case ParameterInfo parameter:
                    return Equals(parameter);

                default:
                    return base.Equals(obj);
            }
        }

        public bool Equals(PropertyInfo other)
        {
            return (null == Target || other?.DeclaringType == Target) &&
                   (null == Type   || other?.PropertyType == Type) &&
                   (null == Name   || other?.Name == Name);
        }

        public bool Equals(ParameterInfo other)
        {
            return (null == Target || other?.Member.DeclaringType == Target) &&
                   (null == Type   || other?.ParameterType == Type) &&
                   (null == Name   || other?.Name == Name);
        }

        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IBuildContext
        {
            if (Value is IResolverPolicy policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory factory)
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
