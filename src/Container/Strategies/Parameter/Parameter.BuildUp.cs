using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class ParameterStrategy<TMemberInfo>
    {
        protected ImportData BuildParameters<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            if (ImportType.Pipeline == descriptor.ValueData.Type)
                return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                    (ResolveDelegate<TContext>)descriptor.ValueData.Value!), ImportType.Value);


            var parameters = Unsafe.As<TMemberInfo>(descriptor.MemberInfo!).GetParameters();
            if (0 == parameters.Length) return new ImportData(EmptyParametersArray, ImportType.Value);

            var arguments = ImportType.Arguments == descriptor.ValueData.Type
                ? BuildParameters(ref context, parameters, (object?[])descriptor.ValueData.Value!)
                : BuildParameters(ref context, parameters);

            return new ImportData(arguments, ImportType.Value);
        }







        protected object?[] BuildParameters<TContext>(ref TContext context, ParameterInfo[] parameters, object?[] data)
            where TContext : IBuilderContext
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<TContext, ParameterInfo>(parameters[index]);
                import.ValueData = Build(ref context, ref import, data[index]); 

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);


                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                if (@override is not null) import.ValueData[ImportType.Dynamic] = @override.Value;

                var finalData = base.BuildUp(ref context, ref import);

                if (context.IsFaulted) return arguments;

                arguments[index] = !finalData.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : finalData.Value;



                //try
                //{

                //    while (ImportType.Dynamic == import.ValueData.Type)
                //        Analyse(ref context, ref import);
                //}
                //catch (Exception ex)
                //{
                //    import.Pipeline = (ResolveDelegate<TContext>)((ref TContext context) => context.Error(ex.Message));
                //}

                //// Use override if provided
                //if (0 < context.Overrides.Length && null != (@override = context.GetOverride<ParameterInfo, MemberDescriptor<ParameterInfo>>(ref import)))
                //    import.ValueData = Build(ref context, ref import, @override.Value);

                //var result = import.ValueData.Type switch
                //    {
                //        ImportType.None => FromContainer(ref context, ref import),
                //        ImportType.Value => import.ValueData,
                //        ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
                //        ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                //        _ => default
                //    }; ;


                //if (context.IsFaulted) return arguments;

                //// TODO: requires optimization
                //arguments[index] = !result.IsValue && import.AllowDefault
                //    ? GetDefaultValue(import.MemberType)
                //    : result.Value;
            }

            return arguments;
        }

        protected object?[] BuildParameters<TContext>(ref TContext context, ParameterInfo[] parameters)
            where TContext : IBuilderContext
        {
            object?[] arguments = new object?[parameters.Length];

            for (var index = 0; index < arguments.Length; index++)
            {
                // Initialize member
                var import = new MemberDescriptor<TContext, ParameterInfo>(parameters[index]);

                ParameterProvider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);

                // Use override if provided
                var @override = context.GetOverride<ParameterInfo, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                if (null != @override) import.ValueData = Build(ref context, ref import, @override.Value);

                var result = import.ValueData.Type switch
                {
                    ImportType.None => FromContainer(ref context, ref import),
                    ImportType.Value => import.ValueData,
                    ImportType.Dynamic => FromUnknown(ref context, ref import, import.ValueData.Value),
                    ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                    _ => default
                }; ;

                if (context.IsFaulted) return arguments;

                // TODO: requires optimization
                arguments[index] = !result.IsValue && import.AllowDefault
                    ? GetDefaultValue(import.MemberType)
                    : result.Value;
            }

            return arguments;
        }


        protected ImportData FromPipeline<TContext>(ref TContext context, ref MemberDescriptor<TContext, ParameterInfo> import, ResolveDelegate<TContext> pipeline)
            where TContext : IBuilderContext => new ImportData(context.FromPipeline(new Contract(import.ContractType, import.ContractName), pipeline), ImportType.Value);

        protected ImportData FromUnknown<TContext>(ref TContext context, ref MemberDescriptor<TContext, ParameterInfo> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportProvider<ParameterInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                        break;

                    case IImportProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, ParameterInfo>>(ref import);
                        break;

                    case IResolve iResolve:
                        return FromPipeline(ref context, ref import, iResolve.Resolve);

                    case ResolveDelegate<TContext> resolver:
                        return FromPipeline(ref context, ref import, resolver);

                    case IResolverFactory<Type> typeFactory:
                        return FromPipeline(ref context, ref import, typeFactory.GetResolver<TContext>(import.MemberType));

                    case PipelineFactory<TContext> factory:
                        return FromPipeline(ref context, ref import, factory(ref context));

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.ContractName = null;
                        import.AllowDefault = false;
                        return FromContainer(ref context, ref import);

                    case UnityContainer.InvalidValue _:
                        return FromContainer(ref context, ref import);

                    default:
                        return new ImportData(data, ImportType.Value);
                }
            }
            while (ImportType.Dynamic == import.ValueData.Type);

            return import.ValueData.Type switch
            {
                ImportType.None => FromContainer(ref context, ref import),
                ImportType.Pipeline => new ImportData(((ResolveDelegate<TContext>)import.ValueData.Value!)(ref context), ImportType.Value),
                _ => import.ValueData
            };
        }


        protected virtual ImportData Build<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportProvider<TMember> provider:
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, TMember>>(ref import);
                        break;

                    case IImportProvider provider:
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, TMember>>(ref import);
                        break;

                    case IResolve iResolve:
                        return new ImportData((ResolveDelegate<BuilderContext>)iResolve.Resolve, ImportType.Pipeline);

                    case ResolveDelegate<BuilderContext> resolver:
                        return new ImportData(resolver, ImportType.Pipeline);

                    case PipelineFactory<TContext> factory:
                        return new ImportData(factory(ref context), ImportType.Pipeline);

                    case IResolverFactory<Type> typeFactory:
                        return new ImportData(typeFactory.GetResolver<BuilderContext>(import.MemberType), ImportType.Pipeline);

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.ContractName = null;
                        import.AllowDefault = false;
                        return default;

                    case UnityContainer.InvalidValue _:
                        return default;

                    default:
                        return new ImportData(data, ImportType.Value);
                }
            }

            while (ImportType.Dynamic == import.ValueData.Type);

            return import.ValueData;
        }
    }
}
