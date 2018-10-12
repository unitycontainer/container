using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
        #region Dynamic Registrations

        private IPolicySet GetDynamicRegistration(Type type, string name)
        {
            var registration = _get(type, name);
            if (null != registration) return registration;

            var info = type.GetTypeInfo();
            return !info.IsGenericType
                ? _root.GetOrAdd(type, name)
                : GetOrAddGeneric(type, name, info.GetGenericTypeDefinition());
        }

        private static object ThrowingBuildUp<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;

            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(ref context);
                }
            }
            catch (Exception ex)
            {
                context.RequiresRecovery?.Recover();
                throw new ResolutionFailedException(context.OriginalBuildKey.Type,
                    context.OriginalBuildKey.Name,
                    CreateMessage(context.OriginalBuildKey.Type,
                        context.OriginalBuildKey.Name,
                        ex, context), ex);
            }

            return context.Existing;
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {

            var registration = new InternalRegistration(type, name);

            if (type.GetTypeInfo().IsGenericType)
            {
                var factory = (InternalRegistration)_get(type.GetGenericTypeDefinition(), name);
                registration.InjectionMembers = factory?.InjectionMembers;
            }

            registration.BuildChain = GetBuilders(registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, string name, Type policyInterface, object policy)
        {
            var registration = new InternalRegistration(type, name, policyInterface, policy);
            registration.BuildChain = GetBuilders(registration);
            return registration;
        }

        #endregion


        #region Resolving Collections

        internal static object ResolveArray<TContext, TElement>(ref TContext context)
            where TContext : IBuilderContext
        {
            var container = (UnityContainer)context.Container;
            var registrations = (IList<InternalRegistration>)GetNamedRegistrations(container, typeof(TElement));

            return ResolveRegistrations<TContext, TElement>(ref context, registrations).ToArray();
        }

        internal static object ResolveGenericArray<TContext, TElement>(ref TContext context, Type type)
            where TContext : IBuilderContext
        {
            var set = new MiniHashSet<InternalRegistration>();
            var container = (UnityContainer)context.Container;
            GetNamedRegistrations(container, typeof(TElement), set);
            GetNamedRegistrations(container, type, set);

            return ResolveGenericRegistrations<TContext, TElement>(ref context, set).ToArray();
        }

        internal static object ResolveEnumerable<TContext, TElement>(ref TContext context)
            where TContext : IBuilderContext
        {
            var container = (UnityContainer)context.Container;
            var registrations = (IList<InternalRegistration>)GetExplicitRegistrations(container, typeof(TElement));

            return ResolveRegistrations<TContext, TElement>(ref context, registrations);
        }

        internal static object ResolveGenericEnumerable<TContext, TElement>(ref TContext context, Type type)
            where TContext : IBuilderContext
        {
            var set = new MiniHashSet<InternalRegistration>();
            var container = (UnityContainer)context.Container;
            GetExplicitRegistrations(container, typeof(TElement), set);
            GetExplicitRegistrations(container, type, set);

            return ResolveGenericRegistrations<TContext, TElement>(ref context, set);
        }


        private static IList<TElement> ResolveGenericRegistrations<TContext, TElement>(ref TContext context, IEnumerable<InternalRegistration> registrations)
            where TContext : IBuilderContext
        {
            var list = new List<TElement>();
            foreach (var registration in registrations)
            {
                try
                {
                    list.Add((TElement)context.NewBuildUp(typeof(TElement), registration.Name));
                }
                catch (Policy.Mapping.MakeGenericTypeFailedException) { /* Ignore */ }
                catch (InvalidOperationException ex)
                {
                    if (!(ex.InnerException is InvalidRegistrationException))
                        throw;
                }
            }

            return list;
        }

        private static IList<TElement> ResolveRegistrations<TContext, TElement>(ref TContext context, IList<InternalRegistration> registrations)
            where TContext : IBuilderContext
        {
            var list = new List<TElement>();
            foreach (var registration in registrations)
            {
                try
                {
                    if (registration.Type.GetTypeInfo().IsGenericTypeDefinition)
                        list.Add((TElement)context.NewBuildUp(typeof(TElement), registration.Name));
                    else
                        list.Add((TElement)context.NewBuildUp(registration));
                }
                catch (ArgumentException ex)
                {
                    if (!(ex.InnerException is TypeLoadException))
                        throw;
                }
            }

            return list;
        }

        private static MiniHashSet<InternalRegistration> GetNamedRegistrations(UnityContainer container, Type type)
        {
            var set = null != container._parent 
                ? GetNamedRegistrations(container._parent, type) 
                : new MiniHashSet<InternalRegistration>();

            if (null == container._registrations) return set;

            var registrations = container.Get(type);
            if (null != registrations && null != registrations.Values)
            {
                var registry = registrations.Values;
                foreach (var entry in registry)
                {
                    if (entry is IContainerRegistration registration &&
                        !string.IsNullOrEmpty(registration.Name))
                        set.Add((InternalRegistration)registration);
                }
            }

            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (generic != type)
            {
                registrations = container.Get(generic);
                if (null != registrations && null != registrations.Values)
                {
                    var registry = registrations.Values;
                    foreach (var entry in registry)
                    {
                        if (entry is IContainerRegistration registration &&
                            !string.IsNullOrEmpty(registration.Name))
                            set.Add((InternalRegistration)registration);
                    }
                }
            }

            return set;
        }

        private static void GetNamedRegistrations(UnityContainer container, Type type, MiniHashSet<InternalRegistration> set)
        {
            if (null != container._parent)
                GetNamedRegistrations(container._parent, type, set);

            if (null == container._registrations) return;

            var registrations = container.Get(type);
            if (registrations?.Values != null)
            {
                var registry = registrations.Values;
                foreach (var entry in registry)
                {
                    if (entry is IContainerRegistration registration &&
                        !string.IsNullOrEmpty(registration.Name))
                        set.Add((InternalRegistration)registration);
                }
            }
        }

        private static MiniHashSet<InternalRegistration> GetExplicitRegistrations(UnityContainer container, Type type)
        {
            var set = null != container._parent
                ? GetExplicitRegistrations(container._parent, type)
                : new MiniHashSet<InternalRegistration>();

            if (null == container._registrations) return set;

            var registrations = container.Get(type);
            if (registrations?.Values != null)
            {
                var registry = registrations.Values;
                foreach (var entry in registry)
                {
                    if (entry is IContainerRegistration registration && string.Empty != registration.Name)
                        set.Add((InternalRegistration)registration);
                }
            }

            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (generic != type)
            {
                registrations = container.Get(generic);
                if (registrations?.Values != null)
                {
                    var registry = registrations.Values;
                    foreach (var entry in registry)
                    {
                        if (entry is IContainerRegistration registration && string.Empty != registration.Name)
                            set.Add((InternalRegistration)registration);
                    }
                }
            }

            return set;
        }

        private static void GetExplicitRegistrations(UnityContainer container, Type type, MiniHashSet<InternalRegistration> set)
        {
            if (null != container._parent)
                GetExplicitRegistrations(container._parent, type, set);

            if (null == container._registrations) return;

            var registrations = container.Get(type);
            if (registrations?.Values != null)
            {
                var registry = registrations.Values;
                foreach (var entry in registry)
                {
                    if (entry is IContainerRegistration registration && string.Empty != registration.Name)
                        set.Add((InternalRegistration)registration);
                }
            }
        }

        #endregion


        #region Exceptions

        public static string CreateMessage(Type typeRequested, string nameRequested, Exception innerException,
                                            IBuilderContext context, string format = Constants.ResolutionFailed)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Resolution of the dependency failed for type = '{typeRequested}', name = '{FormatName(nameRequested)}'.");
            builder.AppendLine($"Exception occurred while: {ExceptionReason(context)}.");
            builder.AppendLine($"Exception is: {innerException?.GetType().GetTypeInfo().Name ?? "ResolutionFailedException"} - {innerException?.Message}");
            builder.AppendLine("-----------------------------------------------");
            builder.AppendLine("At the time of the exception, the container was: ");

            AddContextDetails(builder, context, 1);

            var message = builder.ToString();
            return message;
        }

        private static void AddContextDetails(StringBuilder builder, IBuilderContext context, int depth)
        {
            if (context != null)
            {
                var indentation = new string(' ', depth * 2);
                var key = context.BuildKey;
                var originalKey = context.OriginalBuildKey;

                builder.Append(indentation);

                builder.Append(Equals(key, originalKey)
                    ? $"Resolving {key.Type},{FormatName(key.Name)}"
                    : $"Resolving {key.Type},{FormatName(key.Name)} (mapped from {originalKey.Type}, {FormatName(originalKey.Name)})");

                builder.AppendLine();

                if (context.CurrentOperation != null)
                {
                    builder.Append(indentation);
                    builder.Append(OperationError(context.CurrentOperation.GetType()));
                    builder.AppendLine();
                }

                AddContextDetails(builder, context.ChildContext, depth + 1);
            }
        }

        private static string FormatName(string name)
        {
            return string.IsNullOrEmpty(name) ? "(none)" : name;
        }

        private static string ExceptionReason(IBuilderContext context)
        {
            var deepestContext = context;

            // Find deepest child
            while (deepestContext.ChildContext != null)
            {
                deepestContext = deepestContext.ChildContext;
            }

            // Backtrack to last known operation
            while (deepestContext != context && null == deepestContext.CurrentOperation)
            {
                deepestContext = deepestContext.ParentContext;
            }

            return deepestContext.CurrentOperation != null
                ? OperationError(deepestContext.CurrentOperation)
                : Constants.NoOperationExceptionReason;
        }

        private static string OperationError(object operation)
        {
            switch (operation)
            {
                case ConstructorInfo ctor:
                    return $"Calling constructor {ctor}";

                default:
                    return operation.ToString();
            }
        }

        #endregion

    }
}
