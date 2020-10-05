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
            var methods = type.GetMethods(BindingFlags);
            var downstream = builder.Build();

            ///////////////////////////////////////////////////////////////////
            // Check if any methods are available
            if (0 == methods.Length) return downstream;

            int count = 0;
            Span<bool> set = stackalloc bool[methods.Length];
            var invocations = new InvocationInfo<MethodInfo>[methods.Length];

            ///////////////////////////////////////////////////////////////////
            // Initialize injected members
            for (var injected = builder.Context.Registration?.Methods; null != injected; injected = (InjectionMethod?)injected.Next)
            {
                int index;

                if (-1 == (index = injected.SelectFrom(methods)))
                {
                    // TODO: Proper handling?
                    builder.Context.Error($"Injected member '{injected}' doesn't match any MethodInfo on type {type}");
                    return downstream;
                }

                if (set[index]) continue;
                else set[index] = true;

                invocations[count++] = GetInvocationInfo(methods[index], injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < methods.Length; index++)
            {
                if (set[index]) continue;

                var member = methods[index];
                var import = member.GetCustomAttribute(typeof(InjectionMethodAttribute));

                if (null == import) continue;

                set[index] = true;
                invocations[count++] = GetInvocationInfo(methods[index]);
            }

            ///////////////////////////////////////////////////////////////////
            // Validate and create pipeline
            if (0 == count) return downstream;
            if (invocations.Length > count) Array.Resize(ref invocations, count);

            return (ref PipelineContext context) =>
            {
                for (var invocation = 0; invocation < invocations.Length; invocation++)
                {
                    object?[] arguments;
                    ref var method = ref invocations[invocation];

                    if (null == method.Parameters)
                    {
                        arguments = EmptyParametersArray;
                    }
                    else
                    {
                        arguments = new object?[method.Parameters.Length];
                        PipelineContext local;

                        for (var index = 0; index < arguments.Length; index++)
                        {
                            ref var argument = ref arguments[index];
                            ref var parameter = ref method.Parameters[index];

                            if (parameter.AllowDefault)
                            {
                                ErrorInfo error = default;

                                // Local context
                                local = context.CreateContext(ref parameter.Contract, ref error);
                                using var action = local.Start(parameter.Info);

                                argument = local.Resolve(ref parameter);

                                if (local.IsFaulted)
                                {
                                    argument = parameter.Info.HasDefaultValue
                                        ? parameter.Info.DefaultValue
                                        : GetDefaultValue(parameter.Info.ParameterType);
                                }
                            }
                            else
                            {
                                // Local context
                                local = context.CreateContext(ref parameter.Contract);
                                using var action = local.Start(parameter.Info);

                                arguments[index] = local.Resolve(ref parameter);
                            }

                            if (context.IsFaulted) return argument;
                        }
                    }

                    method.Info.Invoke(context.Target, arguments);
                }

                return downstream?.Invoke(ref context);
            };
        }

    }
}
