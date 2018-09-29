using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;
using static Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation.DynamicMethodConstructorStrategy;

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
                    context.OriginalBuildKey.Name, ex, context);
            }

            return context.Existing;
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {
            var registration = new InternalRegistration(type, name);
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
            MiniHashSet<InternalRegistration> set;

            if (null != container._parent)
                set = GetNamedRegistrations(container._parent, type);
            else
                set = new MiniHashSet<InternalRegistration>();

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
    }
}
