using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Fields

        protected readonly UnityContainer Container;
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();

        #endregion


        #region Constructors

        protected ParametersProcessor(IPolicySet policySet, UnityContainer container)
            : base(policySet)
        {
            Container = container;
        }

        #endregion


        #region Overrides

        protected override Type MemberType(TMemberInfo info)
        {
            Debug.Assert(null != info.DeclaringType);
            return info.DeclaringType!;
        }

        #endregion


        #region Expression 

        protected virtual IEnumerable<Expression> CreateParameterExpressions(ParameterInfo[] parameters, object? injectors = null)
        {
            object[]? resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            if (null == resolvers)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var attribute = (DependencyResolutionAttribute)parameter.GetCustomAttribute(typeof(DependencyResolutionAttribute))
                                  ?? DependencyAttribute.Instance; // Parameters are implicitly required dependencies
                    var resolver = attribute.GetResolver<BuilderContext>(parameter);

                    yield return Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context,
                                        BuilderContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);
                }
            }
            else
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var resolver = PreProcessResolver(parameter, resolvers[i]);

                    yield return Expression.Convert(
                                    Expression.Call(BuilderContextExpression.Context,
                                        BuilderContextExpression.ResolveParameterMethod,
                                        Expression.Constant(parameter, typeof(ParameterInfo)),
                                        Expression.Constant(resolver, typeof(object))),
                                    parameter.ParameterType);
                }
            }
        }

        #endregion


        #region Resolution

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var attribute = (DependencyResolutionAttribute)parameter.GetCustomAttribute(typeof(DependencyResolutionAttribute))
                              ?? DependencyAttribute.Instance; // Parameters are implicitly required dependencies
                var name = attribute.Name;
                var resolver = attribute.GetResolver<BuilderContext>(parameter);

                yield return (ref BuilderContext context) => context.Resolve(parameter, resolver);
            }
        }

        protected virtual IEnumerable<ResolveDelegate<BuilderContext>> CreateParameterResolvers(ParameterInfo[] parameters, object injectors)
        {
            object[]? resolvers = null != injectors && injectors is object[] array && 0 != array.Length ? array : null;
            if (null == resolvers)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var attribute = (DependencyResolutionAttribute)parameter.GetCustomAttribute(typeof(DependencyResolutionAttribute))
                                  ?? DependencyAttribute.Instance; // Parameters are implicitly required dependencies
                    var name = attribute.Name;
                    var resolver = attribute.GetResolver<BuilderContext>(parameter);
                    
                    yield return (ref BuilderContext context) => context.Resolve(parameter, resolver);
                }
            }
            else
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var resolver = PreProcessResolver(parameter, resolvers[i]);
                    
                    yield return (ref BuilderContext context) => context.Resolve(parameter, resolver);
                }
            }
        }

        #endregion


        #region Implementation

        private object PreProcessResolver(ParameterInfo parameter, object resolver)
        {
            switch (resolver)
            {
                case IResolve policy:
                    return (ResolveDelegate<BuilderContext>)policy.Resolve;

                case IResolverFactory<ParameterInfo> factory:
                    return factory.GetResolver<BuilderContext>(parameter);

                case Type type:
                    return 
                        typeof(Type) == parameter.ParameterType
                          ? type 
                          : type == parameter.ParameterType 
                              ? FromAttribute(parameter) 
                              : FromType(type);
            }

            return resolver;
        }

        private object FromType(Type type)
        {
            return (ResolveDelegate<BuilderContext>)((ref BuilderContext context) => context.Resolve(type, null));
        }

        private object FromAttribute(ParameterInfo info)
        {
            // By default all parameters are required dependency
            var attribute = (DependencyResolutionAttribute)info.GetCustomAttribute(typeof(DependencyResolutionAttribute))
                          ?? DependencyAttribute.Instance;

            return attribute.GetResolver<BuilderContext>(info);
        }

        protected bool CanResolve(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute;
            
            if (null != attribute) 
                return CanResolve(info.ParameterType, attribute.Name);

            return CanResolve(info.ParameterType, null);
        }

        protected bool CanResolve(Type type, string? name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
#else
            var info = type;
#endif
            if (info.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return Container._isExplicitlyRegistered(type, name) || CanResolve(type!.GetElementType(), name);
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    || !info.DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#else
                    || !type.GetTypeInfo().DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#endif
                    return Container._isExplicitlyRegistered(type, name);

                return true;
            }

            // Can resolve if IEnumerable or factory is registered
            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || Container._isExplicitlyRegistered(genericType, name))
                {
                    return true;
                }
            }

            // Check if Type is registered
            return Container._isExplicitlyRegistered(type, name);
        }

        #endregion



        #region Resolution

        //protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info)
        //{
        //    var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
        //                                                                                   ?? DependencyAttribute.Instance;
        //    var resolver = attribute.GetResolver<BuilderContext>(info);

        //    return (ref BuilderContext context) =>
        //    {
        //        return context.Resolve(info, attribute.Name, resolver);
        //    };
        //}

        //protected virtual ResolveDelegate<BuilderContext> GetResolverDelegate(ParameterInfo info, object? data)
        //{
        //    var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
        //                                                                                   ?? DependencyAttribute.Instance;
        //    ResolveDelegate<BuilderContext>? resolver = data switch
        //    {
        //        IResolve policy                                   => policy.Resolve,
        //        IResolverFactory<ParameterInfo> propertyFactory   => propertyFactory.GetResolver<BuilderContext>(info),
        //        IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<BuilderContext>(info.ParameterType),
        //        Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<BuilderContext>(info),
        //        _                                                 => null
        //    };

        //    if (null == resolver)
        //    {
        //        return (ref BuilderContext context) =>
        //        {
        //            return context.Override(info, attribute.Name, data);
        //        };
        //    }
        //    else
        //    {
        //        return (ref BuilderContext context) =>
        //        {
        //            return context.Resolve(info, attribute.Name, resolver);
        //        };
        //    }
        //}

        #endregion

    }
}
