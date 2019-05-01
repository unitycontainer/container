using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Builder;
using Unity.Factories;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public class FactoryBuilder : PipelineBuilder
    {
        #region PipelineBuilder

        public override IEnumerable<Expression> Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                      Type type, ImplicitRegistration registration)
        {
            yield break;
        }

        public override ResolveDelegate<BuilderContext>? Build(UnityContainer container, IEnumerator<PipelineBuilder> enumerator, 
                                                               Type type, ImplicitRegistration registration, ResolveDelegate<BuilderContext>? seed)
        {
            // Try to get resolver
            var resolver = registration.Get(typeof(ResolveDelegate<BuilderContext>)) ??
                           container.GetPolicy(type, typeof(ResolveDelegate<BuilderContext>));

            if (null == resolver && registration is ExplicitRegistration explicitRegistration)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_0
                if (null != type && type.GetTypeInfo().IsGenericType)
#else
                if (null != type && type.IsGenericType)
#endif
                    resolver = container.GetPolicy(type.GetGenericTypeDefinition(), typeof(ResolveDelegate<BuilderContext>));
            }

            // Process if found
            if (null != resolver) return Pipeline(container, enumerator, type, registration, (ResolveDelegate<BuilderContext>)resolver);


            // Try finding factory
            // From registration
            ResolveDelegateFactory? factory = registration.Get<ResolveDelegateFactory>();
            if (null != factory) return Pipeline(container, enumerator, type, registration, (ref BuilderContext context) => factory(ref context)(ref context));

#if NETCOREAPP1_0 || NETSTANDARD1_0
            if (null != type && type.GetTypeInfo().IsGenericType)
#else
            if (null != type && type.IsGenericType)
#endif
            {
                factory = (ResolveDelegateFactory?)container.GetPolicy(type.GetGenericTypeDefinition(), typeof(ResolveDelegateFactory));
            }
            else if (type.IsArray)
            {
                return Pipeline(container, enumerator, type, registration, (ref BuilderContext context) => ArrayResolver.Factory(ref context)(ref context));
            }

            return null != factory 
                ? Pipeline(container, enumerator, type, registration, (ref BuilderContext context) => factory(ref context))
                : Pipeline(container, enumerator, type, registration, seed);
        }

        #endregion


        #region Implementation

//        public ResolveDelegateFactory GetFactory(UnityContainer container, Type type, ImplicitRegistration Registration)
//        {
//            // From registration

//            if (Registration is ExplicitRegistration explicitRegistration)
//            {
//#if NETCOREAPP1_0 || NETSTANDARD1_0
//                if (null != Type && Type.GetTypeInfo().IsGenericType)
//#else
//                if (null != type && type.IsGenericType)
//#endif
//                {
//                    return container.GetFactoryPolicy(type.GetGenericTypeDefinition(), Name) ??
//                           container.Defaults.ResolveDelegateFactory;
//                }
//                else if (RegistrationType.IsArray) return ArrayResolver.Factory;
//            }
//            else
//            {
//#if NETCOREAPP1_0 || NETSTANDARD1_0
//                if (RegistrationType.GetTypeInfo().IsGenericType)
//#else
//                if (RegistrationType.IsGenericType)
//#endif
//                {
//                    return ((UnityContainer)Container).GetFactoryPolicy(RegistrationType.GetGenericTypeDefinition(), Name) ??
//                           ((UnityContainer)Container).Defaults.ResolveDelegateFactory;
//                }
//                else if (RegistrationType.IsArray) return ArrayResolver.Factory;
//            }

//            return ((UnityContainer)Container).GetFactoryPolicy(Type) ??
//                   ((UnityContainer)Container).Defaults.ResolveDelegateFactory;
//        }

        #endregion
    }
}
