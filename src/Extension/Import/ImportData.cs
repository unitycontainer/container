using System.Diagnostics;

namespace Unity.Extension
{
    [DebuggerDisplay("Import: {Type},  Data: {Value}")]
    public struct ImportData
    {
        #region Fields

        public object? Value;
        public ImportType Type;

        #endregion


        #region Constructors

        public ImportData(object? data, ImportType type = ImportType.Unknown)
        {
            Value = data;
            Type = type;
        }

        #endregion


        #region Indexer

        public object? this[ImportType type]
        {
            set
            {
                Type = type;
                Value = value;
            }
        }


        #endregion


        #region Convenience Accessors

        public bool IsNone => ImportType.None == Type;

        public bool IsValue => ImportType.Value == Type;
        
        public bool IsUnknown => ImportType.Unknown == Type;

        public bool IsPipeline => ImportType.Pipeline == Type;

        #endregion
    }
}
