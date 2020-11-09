using System;
using Unity.Resolution;

namespace Unity.Container
{
    public struct ReflectionInfo<TMember>
    {
        #region Fields

        public ImportInfo<TMember> Import;
        public ImportData Data;

        #endregion

        public bool GetReflectionInfo(TMember info, Type type, object? data)
        {
            Import.Member = info;
            Import.ContractType = type;

            var value = data;
            do
            {
                Data = value is IReflectionProvider<TMember> provider
                    ? provider.GetReflectionInfo(ref Import)
                    : info.AsImportData(type, data);
                value = Data.Value;
            }
            while (ImportType.Unknown == Data.ImportType);

            return false;
        }
    }

    public static class ReflectionInfoExtensions
    {
        public static ImportData AsImportData<TMember>(this TMember info, Type type, object? data)
        {
            return data switch
            {
                Type target when typeof(Type) != type       => default,
                IResolve iResolve                           => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver   => new ImportData(resolver, ImportType.Pipeline),
                IResolverFactory<TMember> infoFactory       => new ImportData(infoFactory.GetResolver<PipelineContext>(info), ImportType.Pipeline),
                IResolverFactory <Type> typeFactory         => new ImportData(typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline),
                PipelineFactory factory                     => new ImportData(factory, ImportType.Pipeline),
                RegistrationManager.InvalidValue _          => default,
                _ => new ImportData(data, ImportType.Value),
            };
        }
    }
}
