using System;
using System.Collections;
using Unity.Extension;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        protected ImportData FromContainer<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            ErrorDescriptor error = default;

            var value = descriptor.AllowDefault
                ? context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName), ref error)
                : context.FromContract(new Contract(descriptor.ContractType, descriptor.ContractName));

            if (error.IsFaulted)
            {
                // TODO: Default value
                return descriptor.DefaultData.IsValue
                    ? new ImportData(descriptor.DefaultData.Value, ImportType.Value)
                    : default;
            }

            return new ImportData(value, ImportType.Value);
        }

        protected virtual ImportData FromDynamic<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            switch (descriptor.ValueData.Value)
            {
                case IResolve iResolve:
                    return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                            (ResolveDelegate<TContext>)iResolve.Resolve), ImportType.Value);


                case ResolveDelegate<TContext> resolver:
                    return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                        resolver), ImportType.Value);


                case IResolverFactory<TMember> factory:
                    return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                        factory.GetResolver<TContext>(descriptor.MemberInfo)), ImportType.Value);

                case IResolverFactory<Type> factory:
                    return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                        factory.GetResolver<TContext>(descriptor.MemberType)), ImportType.Value);

                case PipelineFactory<TContext> factory:
                    return new ImportData(context.FromPipeline(new Contract(descriptor.ContractType, descriptor.ContractName),
                        factory(ref context)), ImportType.Value);


                case Type target when typeof(Type) != descriptor.MemberType:
                    descriptor.ContractType = target;
                    descriptor.ContractName = null;
                    descriptor.AllowDefault = false;
                    return FromContainer(ref context, ref descriptor);

                case UnityContainer.InvalidValue _:
                    return FromContainer(ref context, ref descriptor);

                default:
                    return new ImportData(descriptor.ValueData.Value, ImportType.Value);
            }
        }

        protected virtual ImportData FromArguments<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            var data = descriptor.ValueData.Value as object?[];
            var type = descriptor.ContractType.GetElementType();
            if (data is null || type is null) return default;

            IList buffer;

            try
            {
                buffer = Array.CreateInstance(type, data.Length);

                for (var i = 0; i < data.Length; i++)
                {
                    var import = descriptor.With(type, data[i]);

                    var result = import.ValueData.Type switch
                    {
                        ImportType.None => FromContainer(ref context, ref import),
                        ImportType.Value => import.ValueData,
                        ImportType.Arguments => FromArguments(ref context, ref import),

                        _ => FromDynamic(ref context, ref import),
                    };

                    buffer[i] = result.Value;
                }
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
                return default;
            }
            
            return new ImportData(buffer, ImportType.Value);
        }
    }
}
