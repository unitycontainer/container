using System;
using System.Reflection;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that can be passed to
    /// <see cref="IUnityContainer.RegisterType"/> to configure a
    /// parameter or property as an optional dependency.
    /// </summary>
    public class OptionalParameter : ParameterBase,
                                     IResolverFactory<Type>,
                                     IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly string _name;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> instance that
        /// specifies to optionally resolve whatever <see cref="Type"/> specified
        /// at this position
        /// </summary>
        public OptionalParameter()
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> object that
        /// specifies the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        public OptionalParameter(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> object that
        /// specifies the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name for the dependency.</param>
        public OptionalParameter(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter"/> object that
        /// specifies the given <paramref name="type"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="type">Type of the dependency.</param>
        /// <param name="name">Name for the dependency.</param>
        public OptionalParameter(Type type, string name)
            : base(type)
        {
            _name = name;
        }

        #endregion


        #region IResolverFactory

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            return (ref TContext c) =>
            {
                try { return c.Resolve(ParameterType ?? type, _name); }
                catch (Exception ex) 
                when (!(ex is CircularDependencyException))
                {
                    return null;
                }
            };
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext
        {
#if NET40
            object value = null;
#else
            var value = info.HasDefaultValue ? info.DefaultValue : null;
#endif
#if NETSTANDARD1_0 || NETCOREAPP1_0 
            var typeInfo = ParameterType?.GetTypeInfo();
            if (null == typeInfo || typeInfo.IsGenericType && typeInfo.ContainsGenericParameters ||
                ParameterType.IsArray && ParameterType.GetElementType().GetTypeInfo().IsGenericParameter ||
                ParameterType.IsGenericParameter)
#else
            if (null == ParameterType || ParameterType.IsGenericType && ParameterType.ContainsGenericParameters ||
                ParameterType.IsArray && ParameterType.GetElementType().IsGenericParameter ||
                ParameterType.IsGenericParameter)
#endif
            {
                var type = info.ParameterType;
                return (ref TContext c) =>
                {
                    try { return c.Resolve(type, _name); }
                    catch (Exception ex) 
                    when (!(ex is CircularDependencyException))
                    {
                        return value;
                    }
                };
            }

            return (ref TContext c) =>
            {
                try { return c.Resolve(ParameterType, _name); }
                catch (Exception ex) 
                when (!(ex is CircularDependencyException))
                {
                    return value;
                }
            };
        }

        #endregion
    }

    /// <summary>
    /// A generic version of <see cref="OptionalParameter"></see> that lets you
    /// specify the type of the dependency using generics syntax.
    /// </summary>
    /// <typeparam name="T">Type of the dependency.</typeparam>
    public class OptionalParameter<T> : OptionalParameter
    {
        /// <summary>
        /// Construct a new <see cref="OptionalParameter{T}"/>.
        /// </summary>
        public OptionalParameter() : base(typeof(T))
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalParameter{T}"/> with the given
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalParameter(string name) : base(typeof(T), name)
        {
        }
    }
}
