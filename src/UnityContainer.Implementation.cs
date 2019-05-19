using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
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
        private readonly UnityContainer _root;
        private readonly UnityContainer? _parent;

        internal readonly ModeFlags ExecutionMode;
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

            // Register with parent
            _parent = parent;
            _root = parent._root;
            _parent.LifetimeContainer.Add(this);

            // Defaults and policies
            LifetimeContainer = new LifetimeContainer(this);
            ExecutionMode = parent._root.ExecutionMode;
            Defaults = _root.Defaults;
            Context = new ContainerContext(this);

            // Dynamic Members
            Register = InitAndAdd;

            // Validators
            ValidateType = _root.ValidateType;
            ValidateTypes = _root.ValidateTypes;
            DependencyResolvePipeline = _root.DependencyResolvePipeline;
        }

        #endregion


        #region Validation

        private Func<Type?, Type?, Type> ValidateType = (Type? typeFrom, Type? typeTo) =>
        {
            return typeFrom ??
                   typeTo ??
                   throw new ArgumentException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must not be 'null'");
        };

        private Func<IEnumerable<Type>?, Type, Type[]?> ValidateTypes = (IEnumerable<Type>? types, Type type) =>
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == types) return null;

            var array = types.Where(t => null != t).ToArray();
            return 0 == array.Length ? null : array;
        };

        #endregion


        #region Diagnostic Validation

        private Type DiagnosticValidateType(Type? typeFrom, Type? typeTo)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoFrom = typeFrom?.GetTypeInfo();
            var infoTo   = typeTo?.GetTypeInfo();
            if (null != infoFrom && !infoFrom.IsGenericType && 
                null != infoTo   && !infoTo.IsGenericType   && !infoFrom.IsAssignableFrom(infoTo))
#else
            if (null != typeFrom && !typeFrom.IsGenericType &&
                null != typeTo && !typeTo.IsGenericType && !typeFrom.IsAssignableFrom(typeTo))
#endif
                throw new ArgumentException($"The type {typeTo} cannot be assigned to variables of type {typeFrom}.");

            return typeFrom ??
                   typeTo ??
                   throw new ArgumentException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");
        }

        private Type[]? DiagnosticValidateTypes(IEnumerable<Type>? types, Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == types) return null;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoTo = type.GetTypeInfo();
#endif
            var array = types.Select(t =>
            {
                if (null == t) throw new ArgumentException($"Enumeration contains null value.", "interfaces");

#if NETSTANDARD1_0 || NETCOREAPP1_0
                              var infoFrom = t?.GetTypeInfo();
                              if (null != infoFrom && !infoFrom.IsGenericType && 
                                  null != infoTo   && !infoTo.IsGenericType   && !infoFrom.IsAssignableFrom(infoTo))
#else
                if (null != t && !t.IsGenericType &&
                    null != type && !type.IsGenericType && !t.IsAssignableFrom(type))
#endif
                    throw new ArgumentException($"The type {type} cannot be assigned to variables of type {t}.");

                return t;
            })
                         .ToArray();
            return 0 == array.Length ? null : array;
        }


        private static Func<Exception, string> CreateMessage = (Exception ex) =>
        {
            return $"Resolution failed with error: {ex.Message}\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";
        };

        private static string CreateDiagnosticMessage(Exception ex)
        {
            const string line = "_____________________________________________________";

            var builder = new StringBuilder();
            builder.AppendLine(ex.Message);
            builder.AppendLine(line);
            builder.AppendLine("Exception occurred while:");

            foreach (DictionaryEntry item in ex.Data)
                builder.AppendLine(DataToString(item.Value));

            return builder.ToString();
        }

        private static string DataToString(object value)
        {
            switch (value)
            {
                case ParameterInfo parameter:
                    return $"    for parameter:  {parameter.Name}";

                case ConstructorInfo constructor:
                    var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"   on constructor:  {constructor.DeclaringType.Name}({ctorSignature})";

                case MethodInfo method:
                    var methodSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"        on method:  {method.Name}({methodSignature})";

                case PropertyInfo property:
                    return $"    for property:   {property.Name}";

                case FieldInfo field:
                    return $"       for field:   {field.Name}";

                case Type type:
                    return $"\n• while resolving:  {type.Name}";

                case Tuple<Type, string?> tuple:
                    return $"\n• while resolving:  {tuple.Item1.Name} registered with name: {tuple.Item2}";

                case Tuple<Type, Type> tuple:
                    return $"        mapped to:  {tuple.Item1?.Name}";
            }

            return value.ToString();
        }

        #endregion


        #region BuilderContext

        internal BuilderContext.ResolvePlanDelegate DependencyResolvePipeline { get; set; } =
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
                        throw new CircularDependencyException(thisContext.Type, thisContext.Name);

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    ContainerContext = thisContext.ContainerContext,
                    Registration = thisContext.Registration,
                    IsAsync = thisContext.IsAsync,
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
    }
}
