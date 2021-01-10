using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override ResolveDelegate<TContext>? Build<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        {
            var members = GetDeclaredMembers(context.Type);

            // Skip build if no members
            if (0 == members.Length) return builder.Build(ref context);

            // Load descriptor from attributes
            var imports = new ImportDescriptor<TDependency>[members.Length];
            for (var i = 0; i < members.Length; i++)
            {
                ref var import = ref imports[i];

                import.MemberInfo = Unsafe.As<TDependency>(members[i]);

                DescribeMember(ref import);
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
                    import.IsImport = true;

                    segment.DescribeImport(ref import);
                }
            }

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
