using System;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        private static MemberDescriptor<TMemberInfo>[]? _empty;

        protected MemberDescriptor<TMemberInfo>[] AnalyseType<TContext>(Type type, InjectionMember<TMemberInfo, TData>? injected)
            where TContext : IBuilderContext
        {
            var members = GetDeclaredMembers(type);
            if (0 == members.Length) return _empty ??= new MemberDescriptor<TMemberInfo>[0];

            var imports = new MemberDescriptor<TMemberInfo>[members.Length];

            // Load descriptor from metadata
            for (var i = 0; i < members.Length; i++)
            {
                ref var import = ref imports[i];

                import.MemberInfo = members[i];

                DescribeImport(ref import);
            }

            if (injected is null) return imports;

            // Add injection data
            int index;
            Span<bool> sockets = stackalloc bool[members.Length];

            for (var member = injected;
                     member is not null;
                     member = (InjectionMember<TMemberInfo, TData>?)member.Next)
            {

                // TODO: Validation
                if (-1 == (index = IndexFromInjected(member, members)) || sockets[index])
                    continue;

                sockets[index] = true;
                ref var descriptor = ref imports[index];

                if (!descriptor.IsImport) descriptor.IsImport = true;

                InjectImport<TContext>(ref descriptor, member);
            }

            return imports;
        }


        protected virtual void InjectImport<TContext>(ref MemberDescriptor<TMemberInfo> import, InjectionMember<TMemberInfo, TData> member)
            where TContext : IBuilderContext
        {
            member.DescribeImport(ref import);

            while (ImportType.Dynamic == import.ValueData.Type)
            {
                switch (import.ValueData.Value)
                {
                    case IImportDescriptionProvider<TMemberInfo> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        import.ValueData[ImportType.Pipeline] = (ResolveDelegate<TContext>)iResolve.Resolve;
                        return;

                    case ResolveDelegate<TContext> resolver:
                        import.ValueData[ImportType.Pipeline] = resolver;
                        return;

                    case IResolverFactory<Type> typeFactory:
                        import.ValueData[ImportType.Pipeline] = typeFactory.GetResolver<TContext>(import.MemberType);
                        return;

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.ContractName = null;
                        import.AllowDefault = false;
                        import.ValueData = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        import.ValueData = default;
                        return;

                    default:
                        import.ValueData.Type = ImportType.Value;
                        return;
                }
            }
        }


        protected virtual void InjectImport<TMember, TContext>(ref MemberDescriptor<TMember> import, IImportDescriptionProvider member)
            where TContext : IBuilderContext
        {
            member.DescribeImport(ref import);

            while (ImportType.Dynamic == import.ValueData.Type)
            {
                switch (import.ValueData.Value)
                {
                    case IImportDescriptionProvider<TMember> provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        import.ValueData.Type = ImportType.None;
                        provider.DescribeImport(ref import);
                        break;

                    case IResolve iResolve:
                        import.ValueData[ImportType.Pipeline] = (ResolveDelegate<TContext>)iResolve.Resolve;
                        return;

                    case ResolveDelegate<TContext> resolver:
                        import.ValueData[ImportType.Pipeline] = resolver;
                        return;

                    case IResolverFactory<Type> typeFactory:
                        import.ValueData[ImportType.Pipeline] = typeFactory.GetResolver<TContext>(import.MemberType);
                        return;

                    case Type target when typeof(Type) != import.MemberType:
                        import.ContractType = target;
                        import.ContractName = null;
                        import.AllowDefault = false;
                        import.ValueData = default;
                        return;

                    case UnityContainer.InvalidValue _:
                        import.ValueData = default;
                        return;

                    default:
                        import.ValueData.Type = ImportType.Value;
                        return;
                }
            }
        }
    }
}
