using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Builder;
using Unity.Events;
using Unity.Exceptions;
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

        const string LifetimeManagerInUse = "The lifetime manager is already registered. WithLifetime managers cannot be reused, please create a new one.";
        internal static readonly ResolveDelegate<BuilderContext> DefaultResolver = (ref BuilderContext c) => c.Existing;
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private const int CollisionsCutPoint = 5;

        #endregion


        #region Fields

        // Essentials
        private Registry<int[]>? _metadata;
        private Registry<IPolicySet>? _registry;
        private readonly UnityContainer? _root;
        private readonly UnityContainer? _parent;

        internal readonly DefaultPolicies Defaults;
        internal readonly ContainerContext Context;
        internal readonly LifetimeContainer LifetimeContainer;

        private List<IUnityContainerExtensionConfigurator>? _extensions;

        // Events
        private event EventHandler<RegisterEventArgs> Registering;
        private event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

        // Dynamic Members
        private Func<Type, string?, ImplicitRegistration, ImplicitRegistration?> Register;

        #endregion


        #region Constructor

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Validate input
            Debug.Assert(null != parent, nameof(parent));
            Debug.Assert(null != parent._root, nameof(parent._root));

            // Parent
            _parent = parent;
            _root = parent._root;

            // Register with parent
            _parent.LifetimeContainer.Add(this);

            // Context and policies
            LifetimeContainer = new LifetimeContainer(this);
            Defaults = _root.Defaults;
            Context = new ContainerContext(this);

            // Dynamic Members
            Register = InitAndAdd;

            // Validators
            ValidateType = _root.ValidateType;
            ValidateTypes = _root.ValidateTypes;
        }

        #endregion


        #region BuilderContext

        internal static BuilderContext.ResolvePlanDelegate DependencyResolvePipeline { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        private static object ValidatingDependencyResolvePipeline(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
        {
            if (null == resolver) throw new ArgumentNullException(nameof(resolver));
#if NET40
            return resolver(ref thisContext);
#else
            unsafe
            {
                var parent = thisContext.Parent;
                while (IntPtr.Zero != parent)
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (thisContext.Type == parentRef.Type && thisContext.Name == parentRef.Name)
                        throw new InvalidOperationException($"Circular reference for Type: {thisContext.Type}, Name: {thisContext.Name}",
                            new CircularDependencyException());

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    ContainerContext = thisContext.ContainerContext,
                    Registration = thisContext.Registration,
                    Type = thisContext.Type,
                    List = thisContext.List,
                    Overrides = thisContext.Overrides,
                    DeclaringType = thisContext.Type,
                    Parent = new IntPtr(Unsafe.AsPointer(ref thisContext))
                };

                return resolver(ref context);
            }
#endif
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
