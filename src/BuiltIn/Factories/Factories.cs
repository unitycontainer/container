using System;
using System.Collections.Generic;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = context.Policies;

            // Registered type
            context.Policies.Set<ContainerRegistration, ResolveDelegate<PipelineContext>>(RegisteredAlgorithm);

            // Unregistered type
            context.Policies.Set<ResolveDelegate<PipelineContext>>(UnregisteredAlgorithm);


            // Lazy<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Lazy<>), LazyFactory);

            // Func<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Func<>), FuncFactory);

            // IEnumerable<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(IEnumerable<>), EnumerableFactory);

            // Array
            context.Policies.Set<Array, ResolveDelegate<PipelineContext>>(ArrayFactory);

            // Default array resolution implementation requires a target type selector
            context.Policies.Set<Array, SelectorDelegate<Type, Type>>(ArrayTargetSelector, 
                (_, _, policy) => ArrayTargetSelector = (SelectorDelegate<Type, Type>)(policy ?? 
                    throw new ArgumentNullException(nameof(policy))));
        }


        #region Nested State

        private class State
        {
            public readonly Type[] Types;
            public ResolveDelegate<PipelineContext>? Pipeline;
            public State(params Type[] types) => Types = types;

        }

        #endregion
    }
}
