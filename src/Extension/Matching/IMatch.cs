﻿using System;

namespace Unity.Resolution;


/// <summary>
/// Value used for calculating match rank
/// </summary>
public enum MatchRank : int
{
    /// <summary>
    /// No match
    /// </summary>
    NoMatch = -1,

    /// <summary>
    /// The value is assignable
    /// </summary>
    Compatible = 1,

    /// <summary>
    /// High probability of a match
    /// </summary>
    HigherProspect = 2,

    /// <summary>
    /// Value matches exactly
    /// </summary>
    ExactMatch = 3,
}


/// <summary>
/// Calculates how much member matches
/// </summary>
/// <typeparam name="TOther"><see cref="Type"/> of the target to match to</typeparam>

public interface IMatch<in TOther>
{
    public MatchRank Matches(TOther other);
}
