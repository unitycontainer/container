using System;
using System.Diagnostics;
using Unity.Resolution;

namespace Unity.Container
{
    [DebuggerDisplay("Import: {DataType},  Data: {Value}")]
    public struct ImportData
    {
        #region Fields

        public object? Value;
        public ImportType DataType;

        #endregion


        #region Constructors

        public ImportData(object? data, ImportType type = ImportType.Unknown)
        {
            Value = data;
            DataType = type;
        }

        #endregion

        public static Func<Type, object?, ImportData> ToImportData { get; internal set; }
            = (Type type, object? data) =>
        {
            return data switch
            {
                IResolve iResolve                           => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver   => new ImportData(resolver, ImportType.Pipeline),
                IResolverFactory<Type> typeFactory          => new ImportData(typeFactory.GetResolver<PipelineContext>(type), ImportType.Pipeline),
                PipelineFactory factory                     => new ImportData(factory, ImportType.Pipeline),
                RegistrationManager.InvalidValue _          => default,
                _ => new ImportData(data, ImportType.Value),
            };
        };
    }
}
