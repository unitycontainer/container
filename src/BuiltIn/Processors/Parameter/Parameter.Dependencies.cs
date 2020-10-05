using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        #region Dependencies

        protected object?[] GetDependencies(ref PipelineContext context, DependencyInfo<ParameterInfo>[] parameters)
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
                arguments[index] = GetDependency(ref context, ref parameters[index]);

            return arguments;
        }

        protected object?[] GetDependencies(ref PipelineContext context, ParameterInfo[] parameters, object?[] data)
        {
            if (0 == parameters.Length) return EmptyParametersArray;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = ToDependencyInfo(parameters[index], data[index]);
                arguments[index] = GetDependency(ref context, ref parameter);
            }

            return arguments;
        }

        protected object?[] GetDependencies(ref PipelineContext context, ParameterInfo[] parameters)
        {
            if (0 == parameters.Length) return EmptyParametersArray;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = ToDependencyInfo(parameters[index]);
                arguments[index] = GetDependency(ref context, ref parameter);
            }

            return arguments;
        }

        #endregion


        #region Implementation

        protected object? GetDependency(ref PipelineContext context, ref DependencyInfo<ParameterInfo> parameter)
        {
            object? argument;
            PipelineContext local;
            ResolverOverride? @override;
            
            using var action = context.Start(parameter.Info);

            if (parameter.AllowDefault)
            {
                ErrorInfo error = default;

                // Local context
                local = context.CreateContext(ref parameter.Contract, ref error);

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
                // Local context
                local = context.CreateContext(ref parameter.Contract);

                argument = null != (@override = local.GetOverride(ref parameter))
                    ? local.GetValue(parameter.Info, @override.Value)
                    : local.Resolve(ref parameter);
            }

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        #endregion
    }
}
