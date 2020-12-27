using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Extension
{
    /// <summary>
    /// Represents the context in which a build-up runs.
    /// </summary>
    public interface IBuilderContext : IResolveContext
    {
        #region Policies

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>Any policies added to this object are transient
        /// and will be erased at the end of the buildup.</remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        IPolicies Policies { get; }

        #endregion


        #region Current Request

        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object? Target { get; set; }

        [Obsolete("Property 'Existing' was renamed to 'Target', use Target for new development")]
        object? Existing { get; set; }

        ResolverOverride[] Overrides { get; }

        /// <summary>
        /// Registration associated with current resolution
        /// </summary>
        RegistrationManager? Registration { get; set; }

        #endregion


        #region Error Reporting

        /// <summary>
        /// Returns error condition
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// Report error condition. This condition will be reported when exception
        /// <see cref="ResolutionFailedException"/> is thrown at the end of resolution.
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns><see cref="RegistrationManager.InvalidValue"/> object</returns>
        object Error(string error);

        /// <summary>
        /// Capture provided exception and play it out later
        /// </summary>
        /// <param name="exception">Exception to capture</param>
        /// <returns><see cref="RegistrationManager.InvalidValue"/> object</returns>
        object Capture(Exception exception);

        #endregion


        #region Event Source

        /// <summary>
        /// Currently executed action
        /// </summary>
        object? CurrentOperation { get; set; }

        // TODO: Requires proper placement
        PipelineAction<TAction> Start<TAction>(TAction action) where TAction : class;

        #endregion


        PipelineContext CreateContext(ref Contract contract, ref ErrorInfo error);
        
        PipelineContext CreateContext(ref Contract contract);
    }
}
