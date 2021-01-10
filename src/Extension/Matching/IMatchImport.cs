using System;
using Unity.Extension;

namespace Unity
{
    /// <summary>
    /// Calculates how much member matches the import
    /// </summary>
    public interface IMatchImport
    {
        /// <summary>
        /// Calculates how much member matches the import
        /// </summary>
        /// <typeparam name="TDescriptor"><see cref="Type"/> of the member</typeparam>
        /// <param name="other">The instance of import descriptor</param>
        /// <returns>Returns matching rank</returns>
        public MatchRank MatchImport<TDescriptor>(ref TDescriptor other) 
            where TDescriptor : IImportDescriptor;
    }
}
