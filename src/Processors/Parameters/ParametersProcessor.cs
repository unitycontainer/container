using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();
        
        protected readonly UnityContainer Container;
        
        protected const string InvalidArgument = "Invalid Argument";

        #endregion


        #region Constructors

        protected ParametersProcessor(IPolicySet policySet, UnityContainer container)
            : base(policySet)
        {
            Container = container;
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data) 
            => data switch
        {
            IResolve policy                                   => policy.Resolve,
            IResolverFactory<ParameterInfo> fieldFactory      => fieldFactory.GetResolver<BuilderContext>(info),
            IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<BuilderContext>(info.ParameterType),
            Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<BuilderContext>(type),
            _                                                 => null
        };

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
    }
}
