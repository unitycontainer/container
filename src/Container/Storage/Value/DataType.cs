namespace Unity.Storage;



/// <summary>
/// Enumeration describing different types of stored data
/// </summary>
public enum DataType

{
    /// <summary>
    /// No data
    /// </summary>
    None = 0,

    /// <summary>
    /// The data is an array
    /// </summary>
    Array,

    /// <summary>
    /// The data is a final value
    /// </summary>
    Value,

    /// <summary>
    /// The data is a pipeline
    /// </summary>
    Pipeline,

    /// <summary>
    /// Unknown data, must be analyzed at runtime
    /// </summary>
    Unknown
}
