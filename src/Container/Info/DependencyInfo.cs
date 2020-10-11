using System;
using System.ComponentModel.Composition;

namespace Unity.Container
{
    public struct DependencyInfo<TInfo>
    {
        public TInfo Info;
        public bool AllowDefault;
        public Contract Contract;
        public ImportAttribute? Import;
        public InjectedData Injected;

        public DependencyInfo(TInfo info, Type type, bool allowDefault = false)
        {
            Info = info;
            Import = default;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
            Injected = default;
        }

        public DependencyInfo(TInfo info, Type type, ImportAttribute? import, bool allowDefault = false)
        {
            Info = info;
            Import = import; 
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
            Injected = default;
        }

        public DependencyInfo(TInfo info, Type type, object? data, bool allowDefault = false)
        {
            Info = info;
            Import = default;
            AllowDefault = allowDefault;
            Contract = new Contract(type);
            Injected = Defaults.TranslateData(info, data);
        }

        public DependencyInfo(TInfo info, Type type, ImportAttribute? import, object? data, bool allowDefault = false)
        {
            Info = info;
            Import = import;
            AllowDefault = allowDefault;
            Contract = new Contract(type, import?.ContractName);
            Injected = Defaults.TranslateData(info, data);
        }
    }
}
