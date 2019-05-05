using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Unity.Builder;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    [CLSCompliant(true)]
    [SecuritySafeCritical]
    public partial class UnityContainer
    {
        #region Constants

        internal static readonly ResolveDelegate<BuilderContext> DefaultResolver = (ref BuilderContext c) => c.Existing;
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private const int CollisionsCutPoint = 5;

        #endregion


        #region Fields

        private Registry<IPolicySet>? _registry;
        private Registry<int[]>? _metadata;

        private IPolicySet _validators;

        private Func<Type, string?, ImplicitRegistration, ImplicitRegistration?> Register;

        #endregion


        #region Defaults


        #endregion


        #region Fields

        // Container specific
        private readonly UnityContainer? _root;
        private readonly UnityContainer? _parent;
        private List<IUnityContainerExtensionConfigurator>? _extensions;

        internal readonly DefaultPolicies Defaults;
        internal readonly ContainerContext Context;
        internal readonly LifetimeContainer LifetimeContainer;

        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        #endregion


        #region Constructor

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Parent
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _root   = parent._root ?? throw new ArgumentNullException(nameof(parent));

            // Register with parent
            _parent.LifetimeContainer.Add(this);

            // Context and policies
            LifetimeContainer = new LifetimeContainer(this);
            Defaults = _root.Defaults;
            Context = new ContainerContext(this);

            // Registry
            Register = InitAndAdd;
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
        private struct ContainerRegistrationStruct : IContainerRegistration
        {
            public Type RegisteredType { get; internal set; }

            public string? Name { get; internal set; }

            public Type? MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }
        }

        #endregion
    }
}
