using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up or tear-down operation runs.
    /// </summary>
    public interface IBuilderContext : IResolveContext
    {
        /// <summary>
        /// Gets the <see cref="Unity.Lifetime.ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="Unity.Lifetime.ILifetimeContainer"/> associated with the build.
        /// </value>
        ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// Gets the original build key for the build operation.
        /// </summary>
        /// <value>
        /// The original build key for the build operation.
        /// </value>
        INamedType OriginalBuildKey { get; }

        /// <summary>
        /// GetOrDefault the current build key for the current build operation.
        /// </summary>
        INamedType BuildKey { get; set; }

        /// <summary>
        /// Set of policies associated with this registration
        /// </summary>
        IPolicySet Registration { get; }

        /// <summary>
        /// Reference to Lifetime manager which requires recovery
        /// </summary>
        SynchronizedLifetimeManager RequiresRecovery { get; set; }

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>Any policies added to this object are transient
        /// and will be erased at the end of the buildup.</remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        IPolicyList Policies { get; }

        /// <summary>
        /// The current object being built up or resolved.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object Existing { get; set; }

        /// <summary>
        /// Flag indicating if the build operation should continue.
        /// </summary>
        /// <value>true means that building should not call any more
        /// strategies, false means continue to the next strategy.</value>
        bool BuildComplete { get; set; }

        /// <summary>
        /// An object representing what is currently being done in the
        /// build chain. Used to report back errors if there's a failure.
        /// </summary>
        object CurrentOperation { get; set; }

        /// <summary>
        /// The child build context.
        /// </summary>
        IBuilderContext ChildContext { get; }

        /// <summary>
        /// The parent build context.
        /// </summary>
        IBuilderContext ParentContext { get; }
    }
}
