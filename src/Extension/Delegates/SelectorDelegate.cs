using System;

namespace Unity.Extension
{
    /// <summary>
    /// The selector that depends on the current container's configuration
    /// </summary>
    /// <typeparam name="TScope"><see cref="Type"/> of the scope</typeparam>
    /// <typeparam name="TInput"><see cref="Type"/> of the input</typeparam>
    /// <typeparam name="TOutput"><see cref="Type"/> of the output</typeparam>
    /// <param name="scope">Instance of the scope</param>
    /// <param name="input">Value[s] to select from</param>
    /// <returns>Selected value</returns>
    public delegate TOutput SelectorDelegate<in TScope, in TInput, out TOutput>(TScope scope, TInput input);
}

