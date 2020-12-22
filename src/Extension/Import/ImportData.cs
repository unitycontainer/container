using System.Diagnostics;

namespace Unity.Extension
{
    [DebuggerDisplay("Import: {ImportType},  Data: {Value}")]
    public struct ImportData
    {
        #region Fields

        public object? Value;
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
    }
}
