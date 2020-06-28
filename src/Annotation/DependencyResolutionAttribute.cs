using System;
using System.Reflection;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute,
                                                          IMatchTo<Type>,
                                                          IResolverFactory<Type>,
                                                          IResolverFactory<ParameterInfo>,
                                                          IResolverFactory<PropertyInfo>,
                                                          IResolverFactory<FieldInfo>
    {
        #region Constructors

        protected DependencyResolutionAttribute(string? name)
        {
            Name = name;
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string? Name { get; }

        #endregion


        #region IMatch

        /// <summary>
        /// This method matches type to dependency and checks if it can be resolved
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>Returns true in assumption that any time can be resolved</returns>
        public virtual MatchRank MatchTo(Type type) => MatchRank.ExactMatch;

        #endregion


        #region IResolverFactory

        /// <summary>
        /// <see cref="ParameterInfo"/> resolver factory
        /// </summary>
        /// <<remarks>
        /// <see cref="ParameterInfo"/> resolver will attempt to resolve value for the parameter. 
        /// <para>If resolution succeeds, parameter will be initialized with resolved value. 
        /// If resolution fails, but parameter has default value, that value will be used to 
        /// initialize the parameter.</para>
        /// <para>If no default value is provided, container will either throw 
        /// <see cref="ResolutionFailedException"/> if dependency is required, or apply null 
        /// value if parameter is optional.</para>
        /// </remarks>
        /// <typeparam name="TContext">The context is implicitly determined based on calling method</typeparam>
        /// <param name="info"><see cref="ParameterInfo"/> of the injected parameter</param>
        /// <returns>Returns <see cref="ResolveDelegate{TContext}"/> that resolves dependency</returns>
        public virtual ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info) 
            where TContext : IResolveContext
        {
            if (info.HasDefaultValue)
            {
                return (ref TContext context) =>
                {
                    try
                    {
                        return context.Resolve(info.ParameterType, Name);
                    }
                    catch (Exception ex) when (!(ex.InnerException is CircularDependencyException))
                    {
                        return info.DefaultValue;
                    }
                };
            }
            else
                return GetResolver<TContext>(info.ParameterType);
        }

        /// <summary>
        /// <see cref="PropertyInfo"/> resolver factory
        /// </summary>
        /// <typeparam name="TContext">The context is implicitly determined based on calling method</typeparam>
        /// <param name="info"><see cref="PropertyInfo"/> of the injected property</param>
        /// <returns>Returns <see cref="ResolveDelegate{TContext}"/> that resolves dependency</returns>
        public virtual ResolveDelegate<TContext> GetResolver<TContext>(PropertyInfo info)
            where TContext : IResolveContext => GetResolver<TContext>(info.PropertyType);

        /// <summary>
        /// <see cref="FieldInfo"/> resolver factory
        /// </summary>
        /// <typeparam name="TContext">The context is implicitly determined based on calling method</typeparam>
        /// <param name="info"><see cref="FieldInfo"/> of the injected field</param>
        /// <returns>Returns <see cref="ResolveDelegate{TContext}"/> that resolves dependency</returns>
        public virtual ResolveDelegate<TContext> GetResolver<TContext>(FieldInfo info)
            where TContext : IResolveContext => GetResolver<TContext>(info.FieldType);

        /// <summary>
        /// <see cref="Type"/> resolver factory
        /// </summary>
        /// <typeparam name="TContext">The context is implicitly determined based on calling method</typeparam>
        /// <param name="type"><see cref="Type"/> to resolve</param>
        /// <returns>Returns <see cref="ResolveDelegate{TContext}"/> that resolves dependency</returns>
        public abstract ResolveDelegate<TContext> GetResolver<TContext>(Type type) where TContext : IResolveContext;

        #endregion
    }
}
