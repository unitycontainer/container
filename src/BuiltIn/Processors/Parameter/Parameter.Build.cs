using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        protected object?[] Build(ref PipelineContext context, ParameterInfo[] parameters, object?[] data)
        {
            Debug.Assert(parameters.Length == data.Length);

            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                var info = parameter.AsInjectionInfo(data[index]);

                // Check for override
                arguments[index] = (null != (@override = context.GetOverride(in info.Import)))
                    ? Build(ref context, in info.Import, parameter.AsImportData(@override.Value))
                    : Build(ref context, in info.Import, in info.Data);
            }

            return arguments;
        }

        protected object?[] Build(ref PipelineContext context, ParameterInfo[] parameters)
        {
            ResolverOverride? @override;
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
            {
                var parameter = parameters[index];
                var info = parameter.AsImportInfo();

                // Check for override
                arguments[index] = (null != (@override = context.GetOverride(in info)))
                    ? Build(ref context, in info, parameter.AsImportData(@override.Value))
                    : Build(ref context, in info);
            }

            return arguments;
        }

        protected object? Build(ref PipelineContext context, in ImportInfo<ParameterInfo> import, in ImportData data)
        {
            if (ImportType.Value == data.DataType) return data.Value;

            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = data.DataType switch
            {
                ImportType.None => local.Resolve(),

                ImportType.Pipeline => local.GetValueRecursively(import.Info,
                    ((ResolveDelegate<PipelineContext>)data.Value!).Invoke(ref local)),

                    // TODO: Requires proper handling
                _ => local.Error("Invalid Import Type"),
            };

            if (local.IsFaulted && import.AllowDefault)
            {
                local.Target = import.Info.HasDefaultValue
                    ? import.Info.DefaultValue
                    : GetDefaultValue(import.Info.ParameterType);
            }

            return local.Target;
        }

        protected object? Build(ref PipelineContext context, in ImportInfo<ParameterInfo> import)
        {
            ErrorInfo error = default;
            var contract = import.Contract;
            var local = import.AllowDefault
                ? context.CreateContext(ref contract, ref error)
                : context.CreateContext(ref contract);

            local.Target = local.Resolve();

            if (local.IsFaulted && import.AllowDefault)
            {
                local.Target = import.Info.HasDefaultValue
                    ? import.Info.DefaultValue
                    : GetDefaultValue(import.Info.ParameterType);
            }

            return local.Target;
        }



        #region Dependencies

        protected object?[] GetDependencies(ref PipelineContext context, DependencyInfo<ParameterInfo>[] parameters)
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length && !context.IsFaulted; index++)
                arguments[index] = GetDependency(ref context, ref parameters[index]);

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
                    ? local.GetValueRecursively(parameter.Info, @override.Value)
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
                    ? local.GetValueRecursively(parameter.Info, @override.Value)
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
