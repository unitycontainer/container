using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        public virtual object? Build(ref PipelineContext context, TDependency info, object? data)
        {
            using var action = context.Start(info);

            var attribute = GetImportAttribute(info);
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            // Check if overriden
            var @override = context.GetOverride(info, in contract);
            var value = null == @override ? data : @override.Value;

            // Get value from injected data
            return Build(ref context, ref contract, value);
        }

        public virtual object? Build(ref PipelineContext context, TDependency info)
        {
            using var action = context.Start(info);

            var attribute = GetImportAttribute(info);
            Contract contract = (null == attribute)
                ? new Contract(DependencyType(info))
                : new Contract(attribute.ContractType ?? DependencyType(info), attribute.ContractName);

            // Check if overriden
            var @override = context.GetOverride(info, in contract);
            var value = null == @override ? attribute : @override.Value;

            // Get value from injected data
            return Build(ref context, ref contract, value);
        }

        public virtual object? Build(ref PipelineContext context, ref Contract contract, object? data)
        {


            throw new NotImplementedException();
        }

        private object? InterpretValue(ref PipelineContext context, TDependency info, object? value)
        {
            return value switch
            {
                IResolve iResolve                         => InterpretValue(ref context, info, iResolve.Resolve(ref context)),

                ResolveDelegate<PipelineContext> resolver => InterpretValue(ref context, info, resolver(ref context)),

                IResolverFactory<TDependency> infoFactory => InterpretValue(ref context, info, infoFactory.GetResolver<PipelineContext>(info)
                                                                                                          .Invoke(ref context)),
                IResolverFactory<Type> typeFactory        => InterpretValue(ref context, info, typeFactory.GetResolver<PipelineContext>(context.Type)
                                                                                                          .Invoke(ref context)),
                Type type 
                when typeof(Type) != DependencyType(info) => context.Resolve(type),

                _ => value,
            };
        }
    }
}
