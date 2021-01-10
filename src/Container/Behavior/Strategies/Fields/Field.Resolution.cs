using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public partial class FieldStrategy
    {
        public override ResolveDelegate<TContext>? Build<TBuilder, TContext>(ref TBuilder builder, ref TContext context)
        {
            var members = GetDeclaredMembers(context.Type);

            // Skip build if no members
            if (0 == members.Length) return builder.Build(ref context);

            // Load descriptor from attributes
            var imports = new ImportInfo<FieldInfo>[members.Length];
            for (var i = 0; i < members.Length; i++)
            {
                ref var import = ref imports[i];

                import.MemberInfo = Unsafe.As<FieldInfo>(members[i]);

                DescribeImport(ref import);
            }

            // Add injection data
            var sequence = (context.Registration as ISequenceSegment<InjectionMember<FieldInfo, object>>)?.Next;
            if (sequence is not null)
            {
                int index;
                Span<bool> sockets = stackalloc bool[members.Length];

                for (var segment = sequence;
                         segment is not null;
                         segment = ((ISequenceSegment<InjectionMember<FieldInfo, object>>)segment).Next)
                {

                    // TODO: Validation
                    if (-1 == (index = IndexFromInjected(segment, members)) || sockets[index])
                        continue;

                    ref var import = ref imports[index];
                    sockets[index] = true;
                    import.IsImport = true;

                    segment.DescribeImport(ref import);

                    if (import.ValueData.IsUnknown) import.FromDynamic(ref context, import.ValueData.Value);
                }
            }

            var closure = imports;
            var pipeline = builder.Build(ref context);

            return (ref TContext runtime) =>
            {
                Debug.Assert(runtime.Existing is not null);

                for (var i = 0; i < closure.Length; i++)
                { 
                    // Build members
                }
                
                return pipeline is null ? runtime.Existing : pipeline(ref runtime);
            };
        }
    }
}
