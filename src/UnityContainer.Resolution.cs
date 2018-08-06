using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Policy;
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

        private static object ThrowingBuildUp(IBuilderContext context)
        {
            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;

            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(context);
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

        private static object NotThrowingBuildUp(IBuilderContext context)
        {
            var i = -1;
            var chain = ((InternalRegistration)context.Registration).BuildChain;

            try
            {
                while (!context.BuildComplete && ++i < chain.Count)
                {
                    chain[i].PreBuildUp(context);
                }

                while (--i >= 0)
                {
                    chain[i].PostBuildUp(context);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return context.Existing;
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {
            var registration = new InternalRegistration(type, name);
            registration.BuildChain = GetBuilders(registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, string name, Type policyInterface, IBuilderPolicy policy)
        {
            var registration = new InternalRegistration(type, name, policyInterface, policy);
            registration.BuildChain = GetBuilders(registration);
            return registration;
        }

        #endregion


        #region Resolving Collections

        internal static void ResolveArray<T>(IBuilderContext context)
        {
            var container = (UnityContainer)context.Container;
            var registrations = (IList<InternalRegistration>)GetNamedRegistrations(container, typeof(T));

            context.Existing = ResolveRegistrations<T>(context, registrations).ToArray();
            context.BuildComplete = true;
        }

        internal static void ResolveGenericArray<T>(IBuilderContext context, Type type)
        {
            var set = new MiniHashSet<InternalRegistration>();
            var container = (UnityContainer)context.Container;
            GetNamedRegistrations(container, typeof(T), set);
            GetNamedRegistrations(container, type, set);

            context.Existing = ResolveGenericRegistrations<T>(context, set).ToArray();
            context.BuildComplete = true;
        }
        
        internal static void ResolveEnumerable<T>(IBuilderContext context)
        {
            var container = (UnityContainer)context.Container;
            var registrations = (IList<InternalRegistration>)GetExplicitRegistrations(container, typeof(T));

            context.Existing = ResolveRegistrations<T>(context, registrations);
            context.BuildComplete = true;
        }

        internal static void ResolveGenericEnumerable<T>(IBuilderContext context, Type type)
        {
            var set = new MiniHashSet<InternalRegistration>();
            var container = (UnityContainer)context.Container;
            GetExplicitRegistrations(container, typeof(T), set);
            GetExplicitRegistrations(container, type, set);

            context.Existing = ResolveGenericRegistrations<T>(context, set);
            context.BuildComplete = true;
        }


        private static IList<T> ResolveGenericRegistrations<T>(IBuilderContext context, IEnumerable<InternalRegistration> registrations)
        {
            var list = new List<T>();
            foreach (var registration in registrations)
            {
                try
                {
                    list.Add((T)((BuilderContext)context).NewBuildUp(typeof(T), registration.Name));
                }
                catch (ArgumentException ex)
                {
                    if (!(ex.InnerException is TypeLoadException))
                        throw;
                }
            }

            return list;
        }

        private static IList<T> ResolveRegistrations<T>(IBuilderContext context, IList<InternalRegistration> registrations)
        {
            var list = new List<T>();
            foreach (var registration in registrations)
            {
                try
                {
                    if (registration.Type.GetTypeInfo().IsGenericTypeDefinition)
                        list.Add((T)((BuilderContext)context).NewBuildUp(typeof(T), registration.Name));
                    else
                        list.Add((T)((BuilderContext)context).NewBuildUp(registration));
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
