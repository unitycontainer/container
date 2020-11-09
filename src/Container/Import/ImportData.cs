using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("Import: {DataType},  Data: {Value}")]
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
    }
}
