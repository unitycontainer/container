using System;

namespace Unity.Resolution;


/// <summary>
/// Calculates how much member matches the import
/// </summary>
public interface IMatchContract<TMemberInfo>
{
    /// <summary>
    /// Calculates how much member matches the import
    /// </summary>
    MatchRank RankMatch(TMemberInfo member, Type contractType, string? contractName);
}
