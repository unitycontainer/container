using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Builder
{
    /// <summary>
    /// Interface uniquely identifying named type
    /// </summary>
    public interface IBuildKey
    {
        /// <summary>
        /// Return the <see cref="Type"/> stored in this build key.
        /// </summary>
        /// <value>The type to build.</value>
        Type Type { get; }

        /// <summary>
        /// Returns the name stored in this build key.
        /// </summary>
        /// <remarks>The name to use when building.</remarks>
        string Name { get; }
    }
}
