using System;
using System.Reflection;
using Unity.Builder.Strategy;
using Unity.Exceptions;
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
        /// <see cref="TypeInfo"/> of the <see cref="Type"/> that is being built.
        /// </summary>
        /// <remarks>This is a convenience property used to optimize allocating
        /// and discarding <see cref="TypeInfo"/>. This instance is cashed and
        /// reused throughout the build.</remarks>
        TypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the head of the strategy chain.
        /// </summary>
        /// <returns>
        /// The strategy that's first in the chain; returns null if there are no
        /// strategies in the chain.
        /// </returns>
        IStrategyChain Strategies { get; }

        /// <summary>
        /// Gets the <see cref="ILifetimeContainer"/> associated with the build.
        /// </summary>
        /// <value>
        /// The <see cref="ILifetimeContainer"/> associated with the build.
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

        /// <summary>
        /// A method to do a new buildup operation on an existing context.
        /// </summary>
        /// <param name="type">Type of to build</param>
        /// <param name="name">Name of the type to build</param>
        /// <param name="childCustomizationBlock">A delegate that takes a <see cref="IBuilderContext"/>. This
        /// is invoked with the new child context before the build up process starts. This gives callers
        /// the opportunity to customize the context for the build process.</param>
        /// <returns>Resolved object</returns>
        object NewBuildUp(Type type, string name, Action<IBuilderContext> childCustomizationBlock = null);

        object NewBuildUp(INamedType registration);

    }
}
