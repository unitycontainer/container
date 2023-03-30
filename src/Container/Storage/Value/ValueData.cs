using System.Diagnostics;

namespace Unity.Storage;



[DebuggerDisplay("Import: {Type},  Data: {Value}")]
public struct ValueData

{
    #region Fields

    public object? Value;
    public ValueType Type;

    #endregion


    #region Constructors

    public ValueData(object? data, ValueType type = ValueType.Unknown)
    {
        Value = data;
        Type = type;
    }

    #endregion


    #region Indexer

    public object? this[ValueType type]
    {
        set
        {
            Type = type;
            Value = value;
        }
    }


    #endregion


    #region Convenience Accessors

    public bool IsValue => ValueType.Value == Type;

    #endregion
}
