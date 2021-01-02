using System;

namespace Unity.Extension
{
    /// <summary>
    /// The delegate to define selection handler that depends on the container's configuration
    /// </summary>
    /// <typeparam name="TInput"><see cref="Type"/> of the input</typeparam>
    /// <typeparam name="TOutput"><see cref="Type"/> of the output</typeparam>
    /// <param name="container">Instance of the container</param>
    /// <param name="input">Value[s] to select from</param>
    /// <returns>Selected value</returns>
    public delegate TOutput SelectorDelegate<in TInput, out TOutput>(UnityContainer container, TInput input);
}

