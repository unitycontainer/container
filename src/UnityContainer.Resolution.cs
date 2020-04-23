using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    [SecuritySafeCritical]
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

        private IPolicySet CreateRegistration(Type type, string name, InternalRegistration factory)
        {
            var registration = new InternalRegistration(type, name);

            if (null != factory)
            {
                registration.InjectionMembers = factory.InjectionMembers;
                registration.Map = factory.Map;
                var lifetime = factory.Get(typeof(LifetimeManager));
                if (lifetime is IFactoryLifetimeManager ManagerFactory)
                {
                    var manager = ManagerFactory.CreateLifetimePolicy();
                    registration.Set(typeof(LifetimeManager), manager);
                }
            }

            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {
            var registration = new InternalRegistration(type, name);

            if (type.GetTypeInfo().IsGenericType)
            {
                var factory = (InternalRegistration)_get(type.GetGenericTypeDefinition(), name);
                if (null != factory)
                {
                    registration.InjectionMembers = factory.InjectionMembers;
                    registration.Map = factory.Map;
                }
            }

            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, Type policyInterface, object policy)
        {
            var registration = new InternalRegistration(policyInterface, policy);
            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        #endregion


        #region Resolving Enumerable

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string, InternalRegistration, object> resolve, string name)
        {
            TElement value;

            var set = GetRegistrations(this, typeof(TElement));

            for (var i = 0; i < set.Count; i++)
            {
                try
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    if (set[i].RegisteredType.GetTypeInfo().IsGenericTypeDefinition)
#else
                    if (set[i].RegisteredType.IsGenericTypeDefinition)
#endif
                    {
                        var registration = (InternalRegistration)GetRegistration(typeof(TElement), set[i].Name);
                        value = (TElement)resolve(typeof(TElement), set[i].Name, registration);
                    }
                    else
                        value = (TElement)resolve(typeof(TElement), set[i].Name, set[i].Registration);
                }
                catch (MakeGenericTypeFailedException) { continue; }
                catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                {
                    continue;
                }
                yield return value;
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    var registration = GetRegistration(typeof(TElement), name);
                    value = (TElement)resolve(typeof(TElement), name, (InternalRegistration)registration);
                }
                catch
                {
                    yield break;
                }

                yield return value;
            }
        }

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string, InternalRegistration, object> resolve,
                                                                   Type generic, string name)
        {
            TElement value;

            var set = GetRegistrations(this, typeof(TElement), generic);

            for (var i = 0; i < set.Count; i++)
            {
                try
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    if (set[i].RegisteredType.GetTypeInfo().IsGenericTypeDefinition)
#else
                    if (set[i].Registration is ContainerRegistration && set[i].RegisteredType.IsGenericTypeDefinition)
#endif
                    {
                        var registration = (InternalRegistration)GetRegistration(typeof(TElement), set[i].Name);
                        value = (TElement)resolve(typeof(TElement), set[i].Name, registration);
                    }
                    else
                        value = (TElement)resolve(typeof(TElement), set[i].Name, set[i].Registration);
                }
                catch (MakeGenericTypeFailedException) { continue; }
                catch (ArgumentException ex) when (ex.InnerException is TypeLoadException) { continue; }

                yield return value;
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    var registration = GetRegistration(typeof(TElement), name);
                    value = (TElement)resolve(typeof(TElement), name, (InternalRegistration)registration);
                }
                catch
                {
                    yield break;
                }

                yield return value;
            }
        }

        #endregion


        #region Resolving Collections

        internal static object ResolveArray<TElement>(ref BuilderContext context)
        {
            var type = typeof(TElement);
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;
#else
            var generic = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
#endif
            var set = generic == type ? GetNamedRegistrations((UnityContainer)context.Container, type)
                                      : GetNamedRegistrations((UnityContainer)context.Container, type, generic);
            return ResolveRegistrations<TElement>(ref context, set).ToArray();
        }

        private static IList<TElement> ResolveRegistrations<TElement>(ref BuilderContext context, RegistrationSet registrations)
        {
            var type = typeof(TElement);
            var list = new List<TElement>();
            for (var i = 0; i < registrations.Count; i++)
            {
                ref var entry = ref registrations[i];
                try
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    if (entry.RegisteredType.GetTypeInfo().IsGenericTypeDefinition)
#else
                    if (entry.RegisteredType.IsGenericTypeDefinition)
#endif
                        list.Add((TElement)context.Resolve(type, entry.Name));
                    else
                        list.Add((TElement)context.Resolve(type, entry.Name, entry.Registration));
                }
                catch (MakeGenericTypeFailedException) { /* Ignore */ }
                catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                {
                    // Ignore
                }
            }

            return list;
        }

        #endregion


        #region Resolving Generic Collections

        internal static object ResolveGenericArray<TElement>(ref BuilderContext context, Type type)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;
#else
            var generic = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
#endif
            var set = generic == type ? GetNamedRegistrations((UnityContainer)context.Container, type)
                                      : GetNamedRegistrations((UnityContainer)context.Container, type, generic);

            return ResolveGenericRegistrations<TElement>(ref context, set).ToArray();
        }

        private static IList<TElement> ResolveGenericRegistrations<TElement>(ref BuilderContext context, RegistrationSet registrations)
        {
            var list = new List<TElement>();
            for (var i = 0; i < registrations.Count; i++)
            {
                ref var entry = ref registrations[i];
                try
                {
                    list.Add((TElement)context.Resolve(typeof(TElement), entry.Name));
                }
                catch (MakeGenericTypeFailedException) { /* Ignore */ }
                catch (InvalidOperationException ex) when (ex.InnerException is InvalidRegistrationException)
                {
                    // Ignore
                }
            }

            return list;
        }

        #endregion


        #region Resolve Delegate Factories

        private static ResolveDelegate<BuilderContext> OptimizingFactory(ref BuilderContext context)
        {
            var counter = 3;
            var type = context.Type;
            var registration = context.Registration;
            ResolveDelegate<BuilderContext> seed = null;
            var chain = ((UnityContainer) context.Container)._processorsChain;

            // Generate build chain
            foreach (var processor in chain)
                seed = processor.GetResolver(type, registration, seed);

            // Return delegate
            return (ref BuilderContext c) => 
            {
                // Check if optimization is required
                if (0 == Interlocked.Decrement(ref counter))
                {
#if NET40
                    Task.Factory.StartNew(() => {
#else
                    Task.Run(() => {
#endif
                        // Compile build plan on worker thread
                        var expressions = new List<Expression>();
                        foreach (var processor in chain)
                        {
                            foreach (var step in processor.GetExpressions(type, registration))
                                expressions.Add(step);
                        }

                        expressions.Add(BuilderContextExpression.Existing);

                        var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(
                            Expression.Block(expressions), BuilderContextExpression.Context);

                        // Replace this build plan with compiled
                        registration.Set(typeof(ResolveDelegate<BuilderContext>), lambda.Compile());
                    });
                }

                return seed?.Invoke(ref c);
            };
        }

        internal ResolveDelegate<BuilderContext> CompilingFactory(ref BuilderContext context)
        {
            var expressions = new List<Expression>();
            var type = context.Type;
            var registration = context.Registration;

            foreach (var processor in _processorsChain)
            {
                foreach (var step in processor.GetExpressions(type, registration))
                    expressions.Add(step);
            }

            expressions.Add(BuilderContextExpression.Existing);

            var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(
                Expression.Block(expressions), BuilderContextExpression.Context);

            return lambda.Compile();
        }

        internal ResolveDelegate<BuilderContext> ResolvingFactory(ref BuilderContext context)
        {
            ResolveDelegate<BuilderContext> seed = null;
            var type = context.Type;
            var registration = context.Registration;

            foreach (var processor in _processorsChain)
                seed = processor.GetResolver(type, registration, seed);

            return seed;
        }

        #endregion


        #region Build Plans

        private ResolveDelegate<BuilderContext> ExecutePlan { get; set; } =
            (ref BuilderContext context) =>
            {
                var i = -1;
                BuilderStrategy[] chain = ((InternalRegistration)context.Registration).BuildChain;

                try
                {
                    while (!context.BuildComplete && ++i < chain.Length)
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

                    if (!(ex.InnerException is InvalidRegistrationException) && 
                        !(ex is InvalidRegistrationException) &&
                        !(ex is ObjectDisposedException) && 
                        !(ex is MemberAccessException) && 
                        !(ex is MakeGenericTypeFailedException) &&
                        !(ex is TargetInvocationException))
                        throw;

                    throw new ResolutionFailedException(context.RegistrationType, context.Name, CreateMessage(ex), ex);
                }

                return context.Existing;
            };

        private object ExecuteValidatingPlan(ref BuilderContext context)
        {
            var i = -1;
            BuilderStrategy[] chain = ((InternalRegistration)context.Registration).BuildChain;

            try
            {
                while (!context.BuildComplete && ++i < chain.Length)
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

                ex.Data.Add(Guid.NewGuid(), null == context.Name
                    ? context.RegistrationType == context.Type
                        ? (object)context.Type
                        : new Tuple<Type, Type>(context.RegistrationType, context.Type)
                    : context.RegistrationType == context.Type
                        ? (object)new Tuple<Type, string>(context.Type, context.Name)
                        : new Tuple<Type, Type, string>(context.RegistrationType, context.Type, context.Name));

                var message = CreateDiagnosticMessage(ex);

                throw new ResolutionFailedException( context.RegistrationType, context.Name, message, ex);
            }

            return context.Existing;
        }

        #endregion


        #region BuilderContext

        internal BuilderContext.ExecutePlanDelegate ContextExecutePlan { get; set; } =
            (BuilderStrategy[] chain, ref BuilderContext context) =>
            {
                var i = -1;

                try
                {
                    while (!context.BuildComplete && ++i < chain.Length)
                    {
                        chain[i].PreBuildUp(ref context);
                    }

                    while (--i >= 0)
                    {
                        chain[i].PostBuildUp(ref context);
                    }
                }
                catch when (null != context.RequiresRecovery)
                {
                    context.RequiresRecovery.Recover();
                    throw;
                }

                return context.Existing;
            };

        internal static object ContextValidatingExecutePlan(BuilderStrategy[] chain, ref BuilderContext context)
        {
            var i = -1;
#if !NET40
            var value = GetPerResolveValue(context.Parent, context.RegistrationType, context.Name);
            if (null != value) return value;
#endif
            try
            {
                while (!context.BuildComplete && ++i < chain.Length)
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
                ex.Data.Add(Guid.NewGuid(), null == context.Name
                    ? context.RegistrationType == context.Type 
                        ? (object)context.Type 
                        : new Tuple<Type, Type>(context.RegistrationType, context.Type)
                    : context.RegistrationType == context.Type 
                        ? (object)new Tuple<Type, string>(context.Type, context.Name) 
                        : new Tuple<Type, Type, string>(context.RegistrationType, context.Type, context.Name));
                throw;
            }

            return context.Existing;

#if !NET40
            object GetPerResolveValue(IntPtr parent, Type registrationType, string name)
            {
                if (IntPtr.Zero == parent) return null;

                unsafe
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (registrationType != parentRef.RegistrationType || name != parentRef.Name)
                        return GetPerResolveValue(parentRef.Parent, registrationType, name);

                    var lifetimeManager = (LifetimeManager)parentRef.Get(typeof(LifetimeManager));
                    var result = lifetimeManager?.GetValue();
                    if (LifetimeManager.NoValue != result) return result;

                    throw new InvalidOperationException($"Circular reference for Type: {parentRef.Type}, Name: {parentRef.Name}",
                            new CircularDependencyException(parentRef.Type, parentRef.Name));
                }
            }
#endif
        }

        internal BuilderContext.ResolvePlanDelegate ContextResolvePlan { get; set; } =
            (ref BuilderContext context, ResolveDelegate<BuilderContext> resolver) => resolver(ref context);

        internal static object ContextValidatingResolvePlan(ref BuilderContext thisContext, ResolveDelegate<BuilderContext> resolver)
        {
            if (null == resolver) throw new ArgumentNullException(nameof(resolver));

#if NET40
            return resolver(ref thisContext);
#else
            unsafe
            {
                var parent = thisContext.Parent;
                while(IntPtr.Zero != parent)
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (thisContext.RegistrationType == parentRef.RegistrationType && thisContext.Name == parentRef.Name)
                        throw new CircularDependencyException(thisContext.Type, thisContext.Name);

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    Lifetime = thisContext.Lifetime,
                    Registration = thisContext.Registration,
                    RegistrationType = thisContext.Type,
                    Name = thisContext.Name,
                    Type = thisContext.Type,
                    ExecutePlan = thisContext.ExecutePlan,
                    ResolvePlan = thisContext.ResolvePlan,
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
