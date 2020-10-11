using System.Diagnostics;

namespace Unity.Container
{
    public enum InjectionType
    {
        None = 0,
        Unknown,
        Value,
        Resolver,
        TypeFactory,
    }

    [DebuggerDisplay("Injected: {DataType},  Data: {Data}")]
    public struct InjectedData

    {
        public object?       Data;
        public InjectionType DataType;

        public InjectedData(object? data)
        {
            Data = data;
            DataType = InjectionType.Unknown;
        }

        public InjectedData(object? data, InjectionType type)
        {
            Data = data;
            DataType = type;
        }
    }
}
