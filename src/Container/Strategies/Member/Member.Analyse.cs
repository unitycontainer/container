using System;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override object? Analyze<TContext>(ref TContext context)
        {
            var type = context.Type;
            var members = GetDeclaredMembers(type);

            if (0 == members.Length) return null;

            var descriptors = new MemberDescriptor<TContext, TMemberInfo>[members.Length];

            // Load descriptor from metadata
            for (var i = 0; i < members.Length; i++)
            {
                ref var descriptor = ref descriptors[i];

                descriptor.MemberInfo = members[i];

                ImportProvider.ProvideInfo(ref descriptor);
            }

            Span<int> set = stackalloc int[members.Length];

            // TODO: Sequence

            // Add injection data
            //for (var member = InjectedMembers(context.Registration);
            //         member is not null;
            //         member = (InjectionMember<TMemberInfo, TData>?)member.Next)
            //{

            //    int index = SelectMember(member, members, ref set);
               
            //    if (-1 == index) continue;

            //    ref var descriptor = ref descriptors[index];

            //    descriptor.IsImport = true;

            //    Analyse(ref context, ref descriptor, member);
            //}

            return descriptors;
        }

        protected virtual void Analyse<TContext>(ref TContext context, ref MemberDescriptor<TContext, TMemberInfo> descriptor, InjectionMember<TMemberInfo, TData> member)
            where TContext : IBuilderContext
        {
            member.ProvideInfo(ref descriptor);

            while (ImportType.Unknown == descriptor.ValueData.Type)
                Analyse(ref context, ref descriptor);
        }

        protected void Analyse<TContext, TMember>(ref TContext context, ref MemberDescriptor<TContext, TMember> descriptor)
            where TContext : IBuilderContext
        {
            switch (descriptor.ValueData.Value)
            {
                case IInjectionProvider<TMember> provider:
                    descriptor.ValueData.Type = ImportType.None;
                    provider.ProvideInfo(ref descriptor);
                    break;

                case IInjectionProvider provider:
                    descriptor.ValueData.Type = ImportType.None;
                    provider.ProvideInfo(ref descriptor);
                    break;

                case IResolve iResolve:
                    descriptor.ValueData[ImportType.Pipeline] = (ResolveDelegate<BuilderContext>)iResolve.Resolve;
                    return;

                case ResolveDelegate<BuilderContext> resolver:
                    descriptor.ValueData[ImportType.Pipeline] = resolver;
                    return;

                case PipelineFactory<TContext> factory:
                    descriptor.ValueData[ImportType.Pipeline] = factory(ref context);
                    return;

                case IResolverFactory<Type> typeFactory:
                    descriptor.ValueData[ImportType.Pipeline] = typeFactory.GetResolver<BuilderContext>(descriptor.MemberType);
                    return;

                case Type target when typeof(Type) != descriptor.MemberType:
                    descriptor.ContractType = target;
                    descriptor.ContractName = null;
                    descriptor.AllowDefault = false;
                    descriptor.ValueData = default;
                    return;

                case UnityContainer.InvalidValue _:
                    descriptor.ValueData = default;
                    return;

                default:
                    descriptor.ValueData.Type = ImportType.Value;
                    return;
            }
        }

    }
}
