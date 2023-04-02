using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Storage;




/// <summary>
/// Structure holding contract information
/// </summary>
[DebuggerDisplay("Contract: Type = { Type?.Name }, Name = { Name }")]
[StructLayout(LayoutKind.Sequential)]
public struct ContractEntry
{
    // Do not change sequence

    /// <summary>
    /// Hash code of the contract
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public int HashCode;

    /// <summary>
    /// <see cref="Type"/> of the contract
    /// </summary>
    public Type Type;

    /// <summary>
    /// Name of the contract
    /// </summary>
    public string? Name;
}
