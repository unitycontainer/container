using System;
using System.Diagnostics;
using System.Linq;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override ResolveDelegate<TContext>? BuildPipeline<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        {
            var members = GetDeclaredMembers(context.Type);

            // Skip build if no members
            if (0 == members.Length) return builder.Build(ref context);

            var count = 0;
            var imports = new ImportDescriptor<TMemberInfo>[members.Length];
            for (var i = 0; i < members.Length; i++)
            {
                ref var import = ref imports[i];

                // Load descriptor from metadata
                import.MemberInfo = members[i];
                DescribeMember(ref import);
                
                if (import.IsImport) count++;
            }

            // Add injection data
            var sequence = (context.Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>)?.Next;
            if (sequence is not null)
            {
                int index;
                Span<bool> sockets = stackalloc bool[members.Length];

                for (var segment = sequence;
                         segment is not null;
                         segment = ((ISequenceSegment<InjectionMember<TMemberInfo, TData>>)segment).Next)
                {

                    // TODO: Validation
                    if (-1 == (index = IndexFromInjected(segment, members)) || sockets[index])
                        continue;

                    ref var import = ref imports[index];
                    sockets[index] = true;

                    if (!import.IsImport)
                    { 
                        import.IsImport = true;
                        count++;
                    }

                    segment.DescribeImport(ref import);
                }
            }

            if (count is 0) return builder.Build(ref context);

            var closure = imports;
            var pipeline = builder.Build(ref context);

            return (ref TContext runtime) =>
            {
                Debug.Assert(runtime.Existing is not null);

                for (var i = 0; i < closure.Length; i++)
                {
                    ref var import = ref closure[i];
                    if (!import.IsImport) continue;

                    Build(ref runtime, ref import);
                }

                return pipeline is null ? runtime.Existing : pipeline(ref runtime);
            };
        }
    }
}
