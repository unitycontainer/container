using System;
using System.Diagnostics;
using Unity.Injection;
using Unity.Resolution;

namespace Unity.Container
{
    [DebuggerDisplay("Import: {ImportType},  Data: {Value}")]
    public struct ImportData
    {
        #region Fields

        public object?    Value;
        public ImportType ImportType;

        #endregion


        #region Constructors

        public ImportData(object? data, ImportType type = ImportType.Unknown)
        {
            Value = data;
            ImportType = type;
        }

        #endregion


        #region Properties
        
        public bool IsNone => ImportType.None == ImportType;

        public bool IsValue => ImportType.Value == ImportType;

        public bool IsPipeline => ImportType.Pipeline == ImportType;

        public bool IsUnknown => ImportType.Unknown == ImportType;

        #endregion

        public static void ProcessImport<T>(ref T info, object? value)
            where T : IInjectionInfo
        {
            do
            {
                switch (value)
                {
                    case IInjectionProvider provider:
                        provider.GetImportInfo(ref info);
                        break;

                    case IResolve iResolve:
                        info.Pipeline = iResolve.Resolve;
                        return;

                    case ResolveDelegate<PipelineContext> resolver:
                        info.Pipeline = resolver;
                        return;

                    case IResolverFactory<Type> typeFactory:
                        info.Pipeline = typeFactory.GetResolver<PipelineContext>(info.MemberType);
                        return;

                    case PipelineFactory factory:
                        info.Pipeline = factory(info.MemberType);
                        return;

                    case Type target when typeof(Type) != info.MemberType:
                        info.ContractType = target;
                        return;

                    case RegistrationManager.InvalidValue _:
                        return;

                    default:
                        info.Value = value;
                        return;
                }

                value = info.ImportValue;
            }
            while (ImportType.Unknown == info.ImportType);
        }

    }
}
