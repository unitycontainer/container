using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Unity.Events;
using Unity.Exceptions;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Utility;

namespace Unity
{
    [CLSCompliant(true)]
    [SecuritySafeCritical]
    public partial class UnityContainer
    {
        #region Constants

        const string LifetimeManagerInUse = "The lifetime manager is already registered. WithLifetime managers cannot be reused, please create a new one.";
        internal static readonly ResolveDelegate<PipelineContext> DefaultResolver = (ref PipelineContext c) => c.Existing;
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        private const int CollisionsCutPoint = 5;

        #endregion


        #region Fields

        // Locks
        private readonly object _syncRegistry = new object();
        private readonly object _syncPipeline = new object();

        // Essentials
        private Metadata? _metadata;
        private Registry? _registry;
        private readonly UnityContainer _root;
        private readonly UnityContainer? _parent;

        internal readonly ModeFlags ExecutionMode;
        internal readonly Storage.DefaultPolicies DefaultContainerPolicies;
        internal readonly ContainerContext Context;
        internal readonly LifetimeContainer LifetimeContainer;

        private List<IUnityContainerExtensionConfigurator>? _extensions;

        // Events
        private event EventHandler<RegisterEventArgs>? Registering;
        private event EventHandler<RegisterInstanceEventArgs>? RegisteringInstance;
        private event EventHandler<ChildContainerCreatedEventArgs>? ChildContainerCreated;

        // Dynamic Members
        private Func<Type, string?, ExplicitRegistration, LifetimeManager?> RegisterType { get; set; }

        private Func<Type, string?, LifetimeManager, LifetimeManager?> RegisterInstance { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Create a <see cref="Unity.UnityContainer"/> with the given parent container.
        /// </summary>
        /// <param name="parent">The parent <see cref="Unity.UnityContainer"/>. The current object
        /// will apply its own settings first, and then check the parent for additional ones.</param>
        private UnityContainer(UnityContainer parent)
        {
            // Register with parent
            _parent = parent;
            _root = parent._root;
            _parent.LifetimeContainer.Add(this);

            // Defaults and policies
            LifetimeContainer = new LifetimeContainer(this);
            ExecutionMode = parent._root.ExecutionMode;
            DefaultContainerPolicies = _root.DefaultContainerPolicies;
            Context = new ContainerContext(this);

            // Dynamic Members
            RegisterType     = InitAndAddType;
            RegisterInstance = InitAndAddInstance;

            // Validators
            ValidateType = _root.ValidateType;
            ValidateTypes = _root.ValidateTypes;
            DependencyResolvePipeline = _root.DependencyResolvePipeline;


            /////////////////////////////////////////////////////////////
            // Build Mode

            var build = _root.ExecutionMode.BuildMode();

            PipelineFromRegistration = build switch
            {
                ModeFlags.Activated => PipelineFromRegistrationActivated,
                ModeFlags.Compiled  => PipelineFromRegistrationCompiled,
                _ => (FromRegistration)PipelineFromRegistrationOptimized
            };

            PipelineFromUnregisteredType = build switch
            {
                ModeFlags.Activated => PipelineFromUnregisteredTypeActivated,
                ModeFlags.Compiled  => PipelineFromUnregisteredTypeCompiled,
                _ => (FromUnregistered)PipelineFromUnregisteredTypeOptimized
            };

            PipelineFromOpenGeneric = build switch
            {
                ModeFlags.Activated => PipelineFromOpenGenericActivated,
                ModeFlags.Compiled  => PipelineFromOpenGenericCompiled,
                _ => (FromOpenGeneric) PipelineFromOpenGenericOptimized
            };
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

        // TODO: Optimize null checks
        private Type DiagnosticValidateType(Type? typeFrom, Type? typeTo)
        {
            var type = typeFrom ??
                       typeTo ??
                       throw new ArgumentException(
                           $"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");

            if (null == typeFrom || null == typeTo) return type;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoFrom = typeFrom?.GetTypeInfo();
            var infoTo   = typeTo?.GetTypeInfo();
            if (null != infoTo && null != infoFrom && !infoFrom.IsGenericType && !infoTo.IsGenericType && !infoFrom.IsAssignableFrom(infoTo))
#else
            if (null != typeTo && null != typeFrom && !typeFrom.IsGenericType && !typeTo.IsGenericType && !typeFrom.IsAssignableFrom(typeTo))
#endif
                throw new ArgumentException($"The type {typeTo} cannot be assigned to variables of type {typeFrom}.");

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (null == typeFrom && null != infoTo && infoTo.IsInterface)
#else
            if (null == typeFrom && null != typeTo && typeTo.IsInterface)
#endif
                throw new ArgumentException($"The type {typeTo} is an interface and can not be constructed.");

#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (null != infoFrom && null != infoTo && infoFrom.IsGenericType && infoTo.IsArray && infoFrom.GetGenericTypeDefinition() == typeof(IEnumerable<>))
#else
            if (null != typeFrom && null != typeTo && typeFrom.IsGenericType && typeTo.IsArray && typeFrom.GetGenericTypeDefinition() == typeof(IEnumerable<>))
#endif
                throw new ArgumentException($"Type mapping of IEnumerable<T> to array T[] is not supported.");

            return type;
        }

        // TODO: Check if could return enumerable
        private Type[]? DiagnosticValidateTypes(IEnumerable<Type>? types, Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == types) return null;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var infoTo = type.GetTypeInfo();
#endif
            var array = types?.Select(t =>
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
            .Cast<Type>()
            .ToArray();

            return null == array || 0 == array.Length ? null : array;
        }


        private static Func<Exception, string> CreateErrorMessage = (Exception ex) =>
        {
            return $"Resolution failed with error: {ex.Message}\n\nFor more detailed information run Unity in debug mode: new UnityContainer(ModeFlags.Diagnostic)";
        };

        private static string CreateDiagnosticMessage(Exception ex)
        {
            const string line = "_____________________________________________________";

            var builder = new StringBuilder();
            builder.AppendLine(ex.Message);
            builder.AppendLine(line);
            builder.AppendLine("Exception occurred:");

            if (null == ex.Data) return builder.ToString();

            foreach (DictionaryEntry item in ex.Data!)
                builder.AppendLine(DataToString(item.Value));

            return builder.ToString();
        }

        private static string? DataToString(object value)
        {
            switch (value)
            {
                case ParameterInfo parameter:
                    return $"    for parameter:  {parameter.Name}";

                case ConstructorInfo constructor:
                    var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    return $"   on constructor:  {constructor.DeclaringType?.Name}({ctorSignature})";

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

        internal PipelineContext.ResolvePlanDelegate DependencyResolvePipeline { get; set; } =
            (ref PipelineContext context, ResolveDelegate<PipelineContext> resolver) => resolver(ref context);

        private static object? ValidatingDependencyResolvePipeline(ref PipelineContext thisContext, ResolveDelegate<PipelineContext> resolver)
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
                    var parentRef = Unsafe.AsRef<PipelineContext>(parent.ToPointer());
                    if (thisContext.Type == parentRef.Type && thisContext.Name == parentRef.Name)
                        throw new CircularDependencyException(thisContext.Type, thisContext.Name);

                    parent = parentRef.Parent;
                }

                var context = new PipelineContext
                {
                    ContainerContext = thisContext.ContainerContext,
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
