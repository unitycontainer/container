using System.Diagnostics;

namespace Unity.Storage;



[DebuggerDisplay("Import: {Type},  Data: {Value}")]
public struct ValueData

{
    #region Fields

    public object? Value;
    public DataType Type;

    #endregion


    #region Constructors

    public ValueData(object? data, DataType type = DataType.Unknown)
    {
        Value = data;
        Type = type;
    }

    #endregion


    #region Indexer

    public object? this[DataType type]
    {
        set
        {
            Type = type;
            Value = value;
        }
    }


    #endregion


    #region Convenience Accessors

    public bool IsValue => DataType.Value == Type;

    #endregion
}
