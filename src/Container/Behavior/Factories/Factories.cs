using System;
using System.Collections.Generic;
using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
        where TContext : IBuilderContext
    {
        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            policies.Set<PipelineFactory<TContext>>(typeof(Lazy<>),        LazyFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(Func<>),        FuncFactory);
            policies.Set<ResolveDelegate<TContext>>(typeof(Array),         ArrayFactory);
            policies.Set<PipelineFactory<TContext>>(typeof(IEnumerable<>), EnumerableFactory);
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
