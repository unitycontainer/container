using System;
using System.Reflection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected object?[] Build<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            ImportInfo<ParameterInfo> import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];
                
                // TODO: requires optimization
                if (!IsValid(ref context, import.MemberInfo)) return arguments;

                DescribeImport(ref import);

                // Use override if provided
                if (null != (@override = GetOverride(ref context, in import)))
                    import.Dynamic = @override.Value;
                else
                    import.Dynamic = data[index];

                var result = import.ValueData.Type switch
                {
                    ImportType.Value => import.ValueData,
                    ImportType.None  => FromContainer(ref context, ref import),
                    _                => FromData(ref context, ref import)
                };

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }

        protected object?[] Build<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            ImportInfo<ParameterInfo> import = default;
            ResolverOverride? @override;

            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                import.MemberInfo = parameters[index];

                // TODO: requires optimization
                if (!IsValid(ref context, import.MemberInfo)) return arguments;

                // Get Import descriptor
                DescribeImport(ref import);

                // Use override if provided
                if (null != (@override = GetOverride(ref context, in import)))
                    import.Dynamic = @override.Value;

                var result = import.ValueData.Type switch
                {
                    ImportType.Value => import.ValueData,
                    ImportType.None  => FromContainer(ref context, ref import),
                    _                => FromData(ref context, ref import)
                };

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }


        protected ImportData FromData<TContext>(ref TContext context, ref ImportInfo<ParameterInfo> import)
            where TContext : IBuilderContext
        {
            do
            {
                switch (import.ValueData.Value)
                {
                    case IImportDescriptionProvider<ParameterInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        import.Value = iResolve.Resolve(ref context);
                        return import.ValueData;

                    case ResolveDelegate<TContext> resolver:
                        import.Value = resolver(ref context);
                        return import.ValueData;

                    case IResolverFactory<Type> typeFactory:
                        var fromTypePipeline = typeFactory.GetResolver<TContext>(import.MemberType);
                        import.Value = fromTypePipeline(ref context);
                        return import.ValueData;

                    //case PipelineFactory<TContext> factory:
                    //    var pipeline = typeFactory.GetResolver<TContext>(import.MemberType);
                    //    import.Value = pipeline(ref context);
                    //    return import.ValueData;
                    //    info.Pipeline = factory(info.MemberType);
                    //    return;

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.AllowDefault = false;
                        import.ValueData.Type = ImportType.None;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        import.Value = context.Resolve(import.ContractType, import.ContractName);
                        return import.ValueData;

                    default:
                        import.ValueData.Type = ImportType.Value;
                        return import.ValueData;
                }
            }
            while (ImportType.Unknown == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None => FromContainer(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }
    }
}
