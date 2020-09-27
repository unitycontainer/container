using System;
using System.Diagnostics;
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
    [DebuggerDisplay("OptionalParameter: Type={ParameterType?.Name ?? \"Any\"} Name={_name ?? \"null\"}")]
    public class OptionalParameter : ParameterBase,
                                     IResolverFactory<Type>,
                                     IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly string? _name;

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
            var value = info.HasDefaultValue 
                ? info.DefaultValue 
                : info.ParameterType.IsValueType
                    ? Activator.CreateInstance(info.ParameterType)
                    : null;

            if (IsInvalidParameterType)
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
                try { return c.Resolve(ParameterType!, _name); }
                catch (Exception ex) 
                when (!(ex is CircularDependencyException))
                {
                    return value;
                }
            };
        }

        #endregion


        #region Overrides

        public override string ToString()
        {
            return $"OptionalParameter: Type={ParameterType?.Name ?? "Any"} Name={_name ?? "null"}";
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
