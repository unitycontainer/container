using System;
using System.Reflection;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class MethodProcessor
    {
        public override ResolveDelegate<PipelineContext>? Build(ref Pipeline_Builder<ResolveDelegate<PipelineContext>?> builder)
        {
            Type type = builder.Context.Type;
            var members = GetSupportedMembers(type);
            var downstream = builder.Build();

            // Check if any methods are available
            if (0 == members.Length) return downstream;

            int count = 0;
            Span<bool> set = stackalloc bool[members.Length];
            InvokeInfo[]? invocations = null;

            // Add injected methods
            for (var injected = builder.Context.Registration?.Methods; null != injected; injected = (InjectionMethod?)injected.Next)
            {
                int index;

                if (-1 == (index = injected.SelectFrom(members)))
                {
                    // TODO: Proper handling?
                    builder.Context.Error($"Injected member '{injected}' doesn't match any MethodInfo on type {type}");
                    return downstream;
                }

                if (set[index]) continue;
                else set[index] = true;

                var info = members[index];
                (invocations ??= new InvokeInfo[members.Length])[count++] = ToInvokeInfo(info, injected.Data);
            }

            // Add annotated methods
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var member = members[index];
                var import = member.GetCustomAttribute(typeof(InjectionMethodAttribute));

                if (import is null) continue;
                else set[index] = true;

                var info = members[index];
                (invocations ??= new InvokeInfo[members.Length - index])[count++] = ToInvokeInfo(info);
            }

            // Validate and trim array
            if (0 == count || null == invocations) return downstream;
            if (invocations.Length > count) Array.Resize(ref invocations, count);

            // Create pipeline
            return (ref PipelineContext context) =>
            {
                for (var index = 0; index < invocations.Length && !context.IsFaulted; index++)
                {
                    //ref var method = ref invocations[index];
                    //object?[] arguments;

                    //if (null != method.Parameters)
                    //{
                    //    ResolverOverride? @override;
                    //    arguments = new object?[method.Parameters.Length];

                    //    //for (var i = 0; i < arguments.Length && !context.IsFaulted; i++)
                    //    //{
                    //    //    ref var parameter = ref method.Parameters[i];

                    //    //    // Check for override
                    //    //    arguments[i] = (null != (@override = context.GetOverride(in parameter.Import)))
                    //    //        ? BuildImport(ref context, in parameter.Import, parameter.Import.Member.AsImportData(@override.Value))
                    //    //        : BuildImport(ref context, in parameter.Import, in parameter.Data);
                    //    //}
                    //}
                    //else
                    //    arguments = EmptyParametersArray;

                    //if (!context.IsFaulted)
                    //    method.Info.Invoke(context.Target, arguments);
                }

                return downstream?.Invoke(ref context);
            };
        }
    }
}
