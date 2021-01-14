using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            Debug.Assert(null != context.Existing, "Target should never be null");
            var members = GetDeclaredMembers(context.Type);

            var sequence = context.Registration as ISequenceSegment<InjectionMember<TMemberInfo, TData>>;
            var injection = sequence?.Next;

            if (0 == members.Length) return;

            for (var i = 0; i < members.Length && !context.IsFaulted; i++)
            {
                var import = new ImportDescriptor<TMemberInfo>(members[i]);

                // Load attributes
                DescribeImport(ref import);

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

                        //import.ValueData = FromInjected(ref context, ref import, injection);

                        injection.DescribeImport(ref import);
                        if (import.RequireBuild) 
                            import.ValueData = Build(ref context, ref import, import.ValueData.Value);

                        goto activate;
                    }

                    continue_next: injection = Unsafe.As<InjectionMember<TMemberInfo, TData>>(injection.Next);
                }

                // Attribute
                if (!import.IsImport) goto next;

                activate: Execute(ref context, ref import);

                // Rewind for the next member
                next: injection = sequence?.Next;
            }
        }

        protected virtual ImportData Build<TContext, TMember>(ref TContext context, ref ImportDescriptor<TMember> import, object? data)
            where TContext : IBuilderContext
        {
            do
            {
                switch (data)
                {
                    case IImportDescriptionProvider<TMember> provider:
                        provider.DescribeImport(ref import);
                        break;

                    case IImportDescriptionProvider provider:
                        provider.DescribeImport(ref import);
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
                        import.Contract = new Contract(target);
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


        protected void Build<TContext, TMember>(ref TContext context, ref ImportDescriptor<TMember> import) 
            where TContext : IBuilderContext
        {
            do
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
                        import.ValueData[ImportType.Pipeline] = (ResolveDelegate<BuilderContext>)iResolve.Resolve;
                        return;

                    case ResolveDelegate<BuilderContext> resolver:
                        import.ValueData[ImportType.Pipeline] = resolver;
                        return;

                    case PipelineFactory<TContext> factory:
                        import.ValueData[ImportType.Pipeline] = factory(ref context);
                        return;

                    case IResolverFactory<Type> typeFactory:
                        import.ValueData[ImportType.Pipeline] = typeFactory.GetResolver<BuilderContext>(import.MemberType);
                        return;

                    case Type target when typeof(Type) != import.MemberType:
                        import.Contract = new Contract(target);
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

            while (ImportType.Dynamic == import.ValueData.Type);
        }
    }
}
