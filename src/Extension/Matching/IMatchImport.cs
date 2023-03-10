using System;

namespace Unity.Resolution;


/// <summary>
/// Calculates how much member matches the import
/// </summary>
public interface IMatchImport<TMemberInfo>
{
    /// <summary>
    /// Calculates how much member matches the import
    /// </summary>
    MatchRank Matches(TMemberInfo member, Type contractType, string? contractName);
}
