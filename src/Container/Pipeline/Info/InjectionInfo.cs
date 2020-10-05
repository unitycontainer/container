namespace Unity.Container
{
    public enum InjectionType
    {
        None = 0,
        Value,
        Resolver,
    }

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
