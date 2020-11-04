using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Container
{
    [DebuggerDisplay("Import: {DataType},  Data: {Value}")]
    public struct ImportData
    {
        public object?    Value;
        public ImportType DataType;

        public ImportData(object? data, ImportType type = ImportType.Unknown)
        {
            Value = data;
            DataType = type;
        }
    }


    public static class ImportDataExtensions
    { 
        public static ImportData AsImportData<T>(this T info, Type type, object? data)
        {
            return data switch
            {
                RegistrationManager.InvalidValue _        => default,
                IResolve iResolve                         => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve, ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver => new ImportData(data,                                               ImportType.Pipeline),
                IResolverFactory<T> infoFactory           => new ImportData(infoFactory.GetResolver<PipelineContext>(info),     ImportType.Pipeline),
                IResolverFactory<Type> typeFactory        => new ImportData(typeFactory.GetResolver<PipelineContext>(type),     ImportType.Pipeline),
                _                                         => new ImportData(data,                                               ImportType.Value),
            };
        }

        public static ImportData AsImportData(this ParameterInfo info, object? data)
        {
            return data switch
            {
                RegistrationManager.InvalidValue _          => default,
                IResolve iResolve                           => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve,           ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver   => new ImportData(data,                                                         ImportType.Pipeline),
                IResolverFactory<ParameterInfo> infoFactory => new ImportData(infoFactory.GetResolver<PipelineContext>(info),               ImportType.Pipeline),
                IResolverFactory<Type> typeFactory          => new ImportData(typeFactory.GetResolver<PipelineContext>(info.ParameterType), ImportType.Pipeline),
                _                                           => new ImportData(data,                                                         ImportType.Value),
            };
        }

        public static ImportData AsImportData(this FieldInfo info, object? data)
        {
            return data switch
            {
                RegistrationManager.InvalidValue _          => default,
                IResolve iResolve                           => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve,       ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver   => new ImportData(data,                                                     ImportType.Pipeline),
                IResolverFactory<FieldInfo> infoFactory     => new ImportData(infoFactory.GetResolver<PipelineContext>(info),           ImportType.Pipeline),
                IResolverFactory<Type> typeFactory          => new ImportData(typeFactory.GetResolver<PipelineContext>(info.FieldType), ImportType.Pipeline),
                _                                           => new ImportData(data,                                                     ImportType.Value),
            };
        }

        public static ImportData AsImportData(this PropertyInfo info, object? data)
        {
            return data switch
            {
                RegistrationManager.InvalidValue _          => default,
                IResolve iResolve                           => new ImportData((ResolveDelegate<PipelineContext>)iResolve.Resolve,          ImportType.Pipeline),
                ResolveDelegate<PipelineContext> resolver   => new ImportData(data,                                                        ImportType.Pipeline),
                IResolverFactory<PropertyInfo> infoFactory  => new ImportData(infoFactory.GetResolver<PipelineContext>(info),              ImportType.Pipeline),
                IResolverFactory<Type> typeFactory          => new ImportData(typeFactory.GetResolver<PipelineContext>(info.PropertyType), ImportType.Pipeline),
                _                                           => new ImportData(data,                                                        ImportType.Value),
            };
        }
    }
}
