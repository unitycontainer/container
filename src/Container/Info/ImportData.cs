using System.Diagnostics;

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
}
