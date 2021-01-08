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

            policies.Set<PipelineFactory<TContext>>(typeof(Lazy<>),        Lazy);
            policies.Set<PipelineFactory<TContext>>(typeof(Func<>),        Func);
            policies.Set<ResolveDelegate<TContext>>(typeof(Array),         Array);
            policies.Set<PipelineFactory<TContext>>(typeof(IEnumerable<>), Enumerable);
        }


        #region Nested State

        // TODO: Replace with Func<>
        private class State
        {
            public readonly Type[] Types;
            public ResolveDelegate<TContext>? Pipeline;
            public State(params Type[] types) => Types = types;
        }

        #endregion

    }
}
