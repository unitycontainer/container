using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected InvocationInfo<TMemberInfo> GetInvocationInfo(TMemberInfo info, object[] data)
        {
            var parameters = info.GetParameters();
            if (0 == parameters.Length) return new InvocationInfo<TMemberInfo>(info);

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                ref var dependency = ref dependencies[i];
                var parameter = parameters[i];

                dependency = GetDependencyInfo(parameter, data[i]);
            }

            return new InvocationInfo<TMemberInfo>(info, dependencies);
        }

        protected InvocationInfo<TMemberInfo> GetInvocationInfo(TMemberInfo info)
        {
            var parameters = info.GetParameters();
            if (0 == parameters.Length) return new InvocationInfo<TMemberInfo>(info);

            var dependencies = new DependencyInfo<ParameterInfo>[parameters.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                ref var dependency = ref dependencies[i];
                var parameter = parameters[i];

                dependency = GetDependencyInfo(parameter);
            }

            return new InvocationInfo<TMemberInfo>(info, dependencies);
        }




        protected ResolveDelegate<PipelineContext> CreateArgumentPileline(DependencyInfo<ParameterInfo>[] dependencies)
        {
            return (ref PipelineContext context) =>
            {
                var arguments = new object?[dependencies.Length];
                ResolverOverride? @override;
                PipelineContext local;
                ErrorInfo error;

                for (var i = 0; i < arguments.Length; i++)
                {
                    ref var argument = ref arguments[i];
                    ref var parameter = ref dependencies[i];


                    if (parameter.AllowDefault)
                    {
                        error = new ErrorInfo();
                        local = context.CreateContext(ref parameter.Contract, ref error);
                        
                        using var action = local.Start(parameter.Info);

                        argument = null != (@override = local.GetOverride(ref parameter))
                            ? local.GetValue(parameter.Info, @override.Value)
                            : local.Resolve(ref parameter);

                        if (local.IsFaulted)
                        {
                            argument = parameter.Info.HasDefaultValue
                                ? parameter.Info.DefaultValue
                                : GetDefaultValue(parameter.Info.ParameterType);
                        }
                    }
                    else
                    {
                        local = context.CreateContext(ref parameter.Contract);

                        using var action = local.Start(parameter.Info);
                        argument = null != (@override = local.GetOverride(ref parameter))
                            ? local.GetValue(parameter.Info, @override.Value)
                            : local.Resolve(ref parameter);
                        
                        if (context.IsFaulted) return argument;
                    }
                }

                return arguments;
            };
        }
    }
}
