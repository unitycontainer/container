using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    internal static partial class UnityDefaultBehaviorExtension<TContext>
        where TContext : IBuilderContext
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            #region Algorithms

            policies.Set<ResolveDelegate<TContext>>(typeof(ContainerRegistration),
                                                    RegisteredAlgorithm);    
            policies.Set<ResolveDelegate<TContext>>(UnregisteredAlgorithm);  

            #endregion


            #region Pipeline Factories

            policies.Set<PipelineFactory<TContext>>(PipelineFromRegistrationFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(Type), FromTypeFactory);
            
            policies.Set<Func<IStagedStrategyChain, ResolveDelegate<TContext>>>(
                                                    PipelineFromStagedChainFactory);
            #endregion


            #region Type Factories

            policies.Set<PipelineFactory<TContext>>(typeof(Lazy<>),        LazyFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(Func<>),        FuncFactory);
            policies.Set<ResolveDelegate<TContext>>(typeof(Array),         ArrayFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(IEnumerable<>), EnumerableFactory);

            #endregion


            #region Selection

            // Default Constructor selector
            policies.Set<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(ConstructorSelector);

            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            // These selectors are used by Build strategies to get declared members

            policies.Set<ConstructorInfo, MembersSelector<ConstructorInfo>>(GetConstructors);
            policies.Set<PropertyInfo, MembersSelector<PropertyInfo>>(GetProperties);
            policies.Set<MethodInfo, MembersSelector<MethodInfo>>(GetMethods);
            policies.Set<FieldInfo, MembersSelector<FieldInfo>>(GetFields);

            #endregion
        }

        #region Nested State

        private class State
        {
            public readonly Type[] Types;
            public ResolveDelegate<TContext>? Pipeline;
            public State(params Type[] types) => Types = types;

        }

        #endregion

    }
}
