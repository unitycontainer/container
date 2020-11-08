using System;
using Unity.Resolution;

namespace Unity.Container
{
    public struct ReflectionInfo<TElement>
    {
        #region Fields

        public ImportInfo<TElement> Import;
        public ImportData Data;

        #endregion


        #region Constructors

        public ReflectionInfo(TElement element, Type contractType, string? contractName, bool allowDefault)
        {
            Import = new ImportInfo<TElement>(element, contractType, contractName, allowDefault);
            Data = default;
        }

        #endregion
    }

    public static class ReflectionInfoExtensions
    {
        public static ReflectionInfo<Type> AsInjectionInfo(this Type type, object? data)
        {
            throw new NotImplementedException();
            //return data switch
            //{
            //    IReflectionProvider<Type> provider
            //        => provider.FillReflectionInfo(type),

            //    Type contractType when typeof(Type) != type
            //        => new ReflectionInfo<Type>(type, contractType, null, ImportType.None),

            //    IResolve iResolve
            //        => new ReflectionInfo<Type>(type, type, (ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),

            //    ResolveDelegate<PipelineContext> resolver
            //        => new ReflectionInfo<Type>(type, type, data, ImportType.Pipeline),

            //    IResolverFactory<Type> typeFactory
            //        => new ReflectionInfo<Type>(type, type, typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline),

            //    RegistrationManager.InvalidValue _
            //        => new ReflectionInfo<Type>(type, type, null, ImportType.None),

            //    _ => new ReflectionInfo<Type>(type, type, data, ImportType.Value),
            //};
        }
    }
}
