using System;
using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// Calculates how much member matches the import
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the member</typeparam>
    public interface IMatchImport
    {
        public MatchRank MatchImport<T>(in T other) where T : IImportDescriptor;
    }
}
