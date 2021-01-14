using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override ResolveDelegate<TContext>? BuildPipeline<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        {
            var imports = AnalyseType<TContext>(context.Type, (context.Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>)?.Next);


            // Skip build if no members
            if (0 == imports.Length) return builder.Build(ref context);


            var closure = imports;
            var pipeline = builder.Build(ref context);

            return (ref TContext runtime) =>
            {
                Debug.Assert(runtime.Existing is not null);

                for (var i = 0; i < closure.Length; i++)
                {
                    ref var import = ref closure[i];
                    if (!import.IsImport) continue;

                    Execute(ref runtime, ref import);
                }

                return pipeline is null ? runtime.Existing : pipeline(ref runtime);
            };
        }
    }
}
