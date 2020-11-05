using System;
using Unity.Resolution;

namespace Unity.Container
{
    public readonly struct ReflectionInfo<TElement>
    {
        #region Fields

        public readonly ImportInfo<TElement> Import;
        public readonly ImportData Data;

        #endregion


        #region Constructors

        public ReflectionInfo(TElement element, Type contractType, string? contractName, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType, contractName, allowDefault);
            Data   = new ImportData(data, import);
        }

        public ReflectionInfo(TElement element, Type contractType, string? contractName, bool allowDefault)
        {
            Import = new ImportInfo<TElement>(element, contractType, contractName, allowDefault);
            Data = default;
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = new ImportData(data, import);
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault, object? data)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = element.AsImportData(contractType, data); 
        }

        public ReflectionInfo(TElement element, Type contractType, bool allowDefault)
        {
            Import = new ImportInfo<TElement>(element, contractType, allowDefault);
            Data = default;
        }

        public ReflectionInfo(TElement element, Type contractType, object? data, ImportType import)
        {
            Import = new ImportInfo<TElement>(element, contractType);
            Data = new ImportData(data, import);
        }

        #endregion
    }

    public static class ReflectionInfoExtensions
    {
        public static ReflectionInfo<Type> AsInjectionInfo(this Type type, object? data)
        {
            return data switch
            {
                IReflectionProvider<Type> provider
                    => provider.GetInfo(type),

                Type contractType when typeof(Type) != type
                    => new ReflectionInfo<Type>(type, contractType, null, ImportType.None),

                IResolve iResolve
                    => new ReflectionInfo<Type>(type, type, (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),

                ResolveDelegate<PipelineContext> resolver
                    => new ReflectionInfo<Type>(type, type, data, ImportType.Pipeline),

                IResolverFactory<Type> typeFactory
                    => new ReflectionInfo<Type>(type, type, typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline),

                RegistrationManager.InvalidValue _
                    => new ReflectionInfo<Type>(type, type, null, ImportType.None),

                _ => new ReflectionInfo<Type>(type, type, data, ImportType.Value),
            };
        }
    }
}
