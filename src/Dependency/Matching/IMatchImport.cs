using System;
using Unity.Container;

namespace Unity
{
    /// <summary>
    /// Calculates how much member matches the import
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the member</typeparam>
    public interface IMatchImport<T>
    {
        public MatchRank Match(in ImportInfo<T> other);
    }
}
