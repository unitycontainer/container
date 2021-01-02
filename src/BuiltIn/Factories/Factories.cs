using System;
using System.Collections.Generic;
using Unity.Container;
using Unity.Extension;

namespace Unity.BuiltIn
{
    public static partial class Factories
    {
        /// <summary>
        /// Setup built-in factories and algorithms
        /// </summary>
        /// <param name="context"></param>
        public static void Setup(ExtensionContext context)
        {
            var policies = context.Policies;

            // Registered type
            policies.Set<ContainerRegistration, ResolveDelegate<PipelineContext>>(RegisteredAlgorithm);

            // Unregistered type
            policies.Set<ResolveDelegate<PipelineContext>>(UnregisteredAlgorithm);


            // Lazy<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Lazy<>), LazyFactory);

            // Func<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(Func<>), FuncFactory);

            // IEnumerable<>
            policies.Set<FromTypeFactory<PipelineContext>>(typeof(IEnumerable<>), EnumerableFactory);

            // Array
            policies.Set<Array, ResolveDelegate<PipelineContext>>(ArrayFactory);

            // Default array implementation requires a target type selector
            _targetTypeSelector = policies.Get<Array, SelectorDelegate<Type, Type>>((_, _, policy) 
                => _targetTypeSelector = (SelectorDelegate<Type, Type>)(policy ?? 
                    throw new ArgumentNullException(nameof(policy))))!;

            // Default 'Chain Execution Pipeline' factory
            policies.Set<Func<IStagedStrategyChain, ResolveDelegate<PipelineContext>>>(
                ChainPipelineFactory);
                // TODO: CompiledChainFactory);

            // Pipeline Factories
            Pipelines.Setup(context);
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
