using System;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that can be passed to
    /// <see cref="IUnityContainer.RegisterType"/> to configure a
    /// parameter or property as an optional dependency.
    /// </summary>
    public class OptionalParameter : ParameterBase, 
                                     IResolverFactory, 
                                     IResolverFactory<ParameterInfo>
    {
        private readonly string _name;

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
            : base(null)
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

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            var info = ParameterType.GetTypeInfo();
            var typeToResolve = !(info.IsGenericType && info.ContainsGenericParameters)
                ? ParameterType
                : ParameterType.GetClosedParameterType(type.GetTypeInfo()
                                                           .GenericTypeArguments);
            return (ref TContext c) =>
            {
                try
                {
                    return c.Resolve(typeToResolve, _name);
                }
                catch
                {
                    return null;
                }
            };
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo parameterInfo) 
            where TContext : IResolveContext
        {
            if (null != ParameterType)
            {
                var info = ParameterType.GetTypeInfo();
                var type = parameterInfo.ParameterType;
                var typeToResolve = !(info.IsGenericType && info.ContainsGenericParameters)
                    ? ParameterType
                    : ParameterType.GetClosedParameterType(type.GetTypeInfo()
                                                               .GenericTypeArguments);
                return (ref TContext c) =>
                {
                    try
                    {
                        return c.Resolve(typeToResolve, _name);
                    }
                    catch
                    {
                        return null;
                    }
                };
            }
            else
            {
                var type = parameterInfo.ParameterType;
                return (ref TContext c) =>
                {
                    try
                    {
                        return c.Resolve(type, _name);
                    }
                    catch
                    {
                        return null;
                    }
                };
            }
        }
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
