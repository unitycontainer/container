using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);


            if (0 == members.Length) return;

            int index, current = 0;
            Span<int> set = stackalloc int[members.Length];
            var sequence  = context.OfType<TMemberInfo, TData>();

            // Assign indexes to injected members
            for (var member = sequence;
                     member is not null;
                     member = (InjectionMember<TMemberInfo, TData>?)member.Next)
            {
                current += 1;

                if (-1 == (index = SelectMember(member, members))) 
                    continue;

                set[index] = current;
            }

            // Process members
            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                var import = new MemberDescriptor<TMemberInfo>(members[i]);

                try
                {
                    ImportProvider.ProvideImport<TContext, MemberDescriptor<TMemberInfo>>(ref import);

                    if (0 <= (index = set[i] - 1))  // Add injection, if match found
                    {
                        sequence![index].ProvideImport<TContext, MemberDescriptor<TMemberInfo>>(ref import);

                        while (ImportType.Dynamic <= import.ValueData.Type) Analyse(ref context, ref import);

                        import.IsImport = true;
                    }
                }
                catch (Exception ex)    // Catch errors from custom providers
                {
                    context.Capture(ex);
                    return;
                }

                // Skip if not an import
                if (!import.IsImport) continue;

                try
                {
                    var @override = context.GetOverride<TMemberInfo, MemberDescriptor<TMemberInfo>>(ref import);
                    var finalData = @override is not null
                        ? FromDynamic(ref context, ref import, @override.Value)
                        : import.ValueData.Type switch
                        {
                            ImportType.None     => FromContainer(ref context, ref import),
                            ImportType.Value    => import.ValueData,
                            ImportType.Dynamic  => FromDynamic(ref context, ref import, import.ValueData.Value),
                            ImportType.Pipeline => FromPipeline(ref context, ref import, (ResolveDelegate<TContext>)import.ValueData.Value!), // TODO: Switch to Contract
                            _                   => default
                        };

                    Execute(ref context, ref import, ref finalData);
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
        }

        protected virtual ImportData Build<TContext, TMember>(ref TContext context, ref MemberDescriptor<TMember> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportProvider<TMember> provider:
                        provider.ProvideImport<TContext, MemberDescriptor<TMember>>(ref import);
                        break;

                    case IImportProvider provider:
                        provider.ProvideImport<TContext, MemberDescriptor<TMember>>(ref import);
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
