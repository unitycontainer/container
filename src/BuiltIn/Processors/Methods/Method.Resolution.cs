using System;
using System.Reflection;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor
    {
        public override ResolveDelegate<PipelineContext>? Build(ref PipelineBuilder<ResolveDelegate<PipelineContext>?> builder)
        {
            Type type = builder.Context.Type;
            var members = type.GetMethods(BindingFlags);
            var downstream = builder.Build();

            // Check if any methods are available
            if (0 == members.Length) return downstream;

            int count = 0;
            Span<bool> set = stackalloc bool[members.Length];
            InvokeInfo<MethodInfo>[]? invocations = null;

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
                (invocations ??= new InvokeInfo<MethodInfo>[members.Length])[count++] = injected.GetInvocationInfo(info);
            }

            // Add annotated methods
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var member = members[index];
                var import = member.GetCustomAttribute(typeof(InjectionMethodAttribute));

                if (null == import) continue;
                else set[index] = true;

                var info = members[index];
                var args = ToDependencyArray(info.GetParameters());
                (invocations ??= new InvokeInfo<MethodInfo>[members.Length - index])[count++] = new InvocationInfo<MethodInfo>(info, args);
            }

            // Validate and trim array
            if (0 == count || null == invocations) return downstream;
            if (invocations.Length > count) Array.Resize(ref invocations, count);

            // Create pipeline
            return (ref PipelineContext context) =>
            {
                for (var index = 0; index < invocations.Length; index++)
                {
                    ref var method = ref invocations[index];

                    object?[] arguments = (null == method.Parameters)
                        ? arguments = EmptyParametersArray
                        : GetDependencies(ref context, method.Parameters);

                    method.Info.Invoke(context.Target, arguments);
                }

                return downstream?.Invoke(ref context);
            };
        }
    }
}
