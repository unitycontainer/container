using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected override void Execute<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import)
        {
            ResolverOverride? @override;

            var result = 0 < context.Overrides.Length && null != (@override = context.GetOverride<TMemberInfo, MemberDescriptor<TMemberInfo>>(ref import))
                ? FromUnknown(ref context, ref import, @override.Value)
                : import.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref import),
                    ImportType.Value => import.ValueData,
                    ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
                    ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                    _ => default
                };

            if (context.IsFaulted || !result.IsValue) return;

            try
            {
                Execute(import.MemberInfo, context.Existing!, result.Value);
            }
            catch (ArgumentException ex)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }

        protected override ImportData FromUnknown<TContext>(ref TContext context, ref MemberDescriptor<TMemberInfo> import, object? data)
        {
            ResolverOverride? @override;
            var descriptors = (MemberDescriptor<ParameterInfo>[])data!;

            object?[] arguments = new object?[descriptors.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                ref var descriptor = ref descriptors[index];

                // Use override if provided
                var result = null != (@override = context.GetOverride<ParameterInfo, MemberDescriptor<ParameterInfo>>(ref descriptor))
                    ? Build(ref context, ref descriptor, @override.Value)
                    : descriptor.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref descriptor),
                    ImportType.Value => descriptor.ValueData,
                    ImportType.Dynamic => FromUnknown(ref context, ref descriptor, descriptor.ValueData.Value),
                    ImportType.Pipeline => FromPipeline(ref context, ref descriptor, (ResolveDelegate<TContext>)descriptor.ValueData.Value!), // TODO: Switch to Contract
                    _ => default
                }; ;

                if (context.IsFaulted) return new ImportData(arguments, ImportType.Value);

                // TODO: requires optimization
                arguments[index] = !result.IsValue && descriptor.AllowDefault
                    ? GetDefaultValue(descriptor.MemberType)
                    : result.Value;
            }

            return new ImportData(arguments, ImportType.Value);
        }
    }
}
