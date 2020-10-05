using System.Diagnostics;

namespace Unity.Container
{
    public enum InjectionType
    {
        None = 0,
        Value,
        Resolver,
    }

    [DebuggerDisplay("Injected: {DataType},  Data: {Data}")]
    public struct InjectionInfo
    {
        public object?       Data;
        public InjectionType DataType;

        public InjectionInfo(object? data, InjectionType type)
        {
            Data = data;
            DataType = type;
        }
    }
}
