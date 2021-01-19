using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

            var sequence = context.OfType<TMemberInfo, TData>();
            var injection = sequence;

            if (0 == members.Length) return;

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                var import = new MemberDescriptor<TMemberInfo>(members[i]);

                DescribeImport(ref import); // Load attributes

                // Injection, if exists
                while (null != injection)
                {
                    var match = injection.Match(Unsafe.As<TMemberInfo>(import.MemberInfo));

                    if (MatchRank.NoMatch != match)
                    {
                        if (MatchRank.ExactMatch != match)
                        {
                            if (injection.Data is IMatch<TMemberInfo, MatchRank> iMatch &&
                                iMatch.Match(import.MemberInfo) is MatchRank.NoMatch)
                                goto continue_next;
                                //continue;
                        }

                        try
                        {
                            injection.DescribeImport<TContext, MemberDescriptor<TMemberInfo>>(ref import);

                            while (ImportType.Dynamic == import.ValueData.Type)
                                Analyse(ref context, ref import);
                        }
                        catch (Exception ex)
                        {
                            import.Pipeline = (ResolveDelegate<TContext>)((ref TContext context) => context.Error(ex.Message));
                        }

                        goto activate;
                    }

                    continue_next: injection = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injection.Next);
                }

                // Attribute
                if (!import.IsImport) goto next;

                activate: Execute(ref context, ref import);

                // Rewind for the next member
                next: injection = sequence;
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
                        provider.DescribeImport<TContext, MemberDescriptor<TMember>>(ref import);
                        break;

                    case IImportProvider provider:
                        provider.DescribeImport<TContext, MemberDescriptor<TMember>>(ref import);
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
