using System;
using Unity.Container;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {

        private static ImportType DefaultImportDataParser(ref ImportInfo info, object? value)
        {
            do
            { 
                switch (value)
                {
                    case Type target when typeof(Type) != info.MemberType:
                        return ImportType.None; 

                    case IResolve iResolve:
                        info.Data.Value = (ResolveDelegate<PipelineContext>)iResolve.Resolve;
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case ResolveDelegate<PipelineContext> resolver:
                        info.Data.Value = resolver;
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case IResolverFactory<Type> typeFactory:
                        info.Data.Value = typeFactory.GetResolver<PipelineContext>(info.MemberType);
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case IResolverFactory<TDependency> memberFactory:
                        info.Data.Value = memberFactory.GetResolver<PipelineContext>(info.MemberInfo);
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case PipelineFactory factory:
                        info.Data.Value = factory(info.MemberType);
                        info.Data.ImportType = ImportType.Pipeline;
                        return ImportType.Pipeline;

                    case IInjectionProvider provider:
                        provider.GetImportInfo(ref info);
                        break;

                    case RegistrationManager.InvalidValue _:
                        return ImportType.None;

                    default:
                        info.Data.Value = value;
                        info.Data.ImportType = ImportType.Value;
                        return ImportType.Value;
                }
            }
            while (ImportType.Unknown == info.ImportType);

            return info.ImportType;
        }


        private static ImportData DefaultImportParser(Type type, object? value)
        {
            switch (value)
            {
                case Type target when typeof(Type) != type:
                    return default;

                case IResolve iResolve:
                    return new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline);

                case ResolveDelegate<PipelineContext> resolver:
                    return new ImportData(resolver, ImportType.Pipeline);

                case IResolverFactory<Type> typeFactory:
                    return new ImportData(typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline);

                case PipelineFactory factory:
                    return new ImportData(factory(type), ImportType.Pipeline);

                case RegistrationManager.InvalidValue _:
                    return default;

                default:
                    return new ImportData(value, ImportType.Value);
            }
        }
    }
}
