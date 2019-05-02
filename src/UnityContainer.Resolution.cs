using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Extensions;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
        #region Constants

        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();
        
        #endregion


        #region Check if can resolve

        internal bool CanResolve(Type type, string? name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
#else
            var info = type;
#endif
            if (info.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return IsRegistered(type, name) || CanResolve(type.GetElementType(), name);
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    || !info.DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#else
                    || !type.GetTypeInfo().DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#endif
                    return IsRegistered(type, name);

                return true;
            }

            // Can resolve if IEnumerable or factory is registered
            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || IsRegistered(genericType, name))
                {
                    return true;
                }
            }

            // Check if Type is registered
            return IsRegistered(type, name);
        }

        #endregion

        #pragma warning disable CS8603 // Possible null reference return.
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

        #region Resolving Enumerable

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve, string? name)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hash = typeof(TElement).GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes and iterate over them
                var length = container._metadata.GetEntries<TElement>(hash, out int[]? data);
                if (null != data && null != registry)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];

                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                var registration = (ExplicitRegistration)registry.Entries[index].Value;
                                value = (TElement)resolve(typeof(TElement), registration.Name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    var registration = GetRegistration(typeof(TElement), name);
                    value = (TElement)resolve(typeof(TElement), name, registration);
                }
                catch
                {
                    yield break;
                }

                yield return value;
            }
        }

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve,
                                                                   Type typeDefinition, string? name)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hashCode = typeof(TElement).GetHashCode();
            int hashGeneric = typeDefinition.GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetEntries<TElement>(hashCode, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];

                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                var registration = (ExplicitRegistration)registry.Entries[index].Value;
                                value = (TElement)resolve(typeof(TElement), registration.Name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetEntries(hashGeneric, typeDefinition, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var key = registry.Entries[index].Value is ImplicitRegistration policySet 
                                ? policySet.Name 
                                : null;

                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                int hash = NamedType.GetHashCode(typeof(TElement), key);
                                var registration = container.GetOrAdd(hash, typeof(TElement), key, registry.Entries[index].Value);
                                value = (TElement)resolve(typeof(TElement), key, registration);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidOperationException ex) when (ex.InnerException is InvalidRegistrationException)
                            {
                                continue;
                            }
                            // TODO: Verify if required
                            //catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            //{
                            //    continue;
                            //}

                            yield return value;
                        }
                    }
                }
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    var registration = GetRegistration(typeof(TElement), name);
                    value = (TElement)resolve(typeof(TElement), name, registration);
                }
                catch
                {
                    yield break;
                }

                yield return value;
            }
        }

        #endregion


        #region Resolving Array

        internal Type GetTargetType(Type argType)
        {
            Type next;
            for (var type = argType; null != type; type = next)
            {
                var info = type.GetTypeInfo();
                if (info.IsGenericType)
                {
                    if (IsRegistered(type)) return type;

                    var definition = info.GetGenericTypeDefinition();
                    if (IsRegistered(definition)) return definition;

                    next = info.GenericTypeArguments[0];
                    if (IsRegistered(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType();
                    if (IsRegistered(next)) return next;
                }
                else
                {
                    return type;
                }
            }

            return argType;
        }

        internal IEnumerable<TElement> ResolveArray<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve, Type type)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hash = type.GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes and iterate over them
                var length = container._metadata.GetEntries(hash, type, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Value is ImplicitRegistration policySet 
                                 ? policySet.Name 
                                 : null;

                        if (null == name) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                var registration = (ExplicitRegistration)registry.Entries[index].Value;
                                value = (TElement)resolve(typeof(TElement), name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ResolveArray<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve,
                                                              Type type, Type typeDefinition)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hashCode = type.GetHashCode();
            int hashGeneric = typeDefinition.GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetEntries(hashCode, type, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Value is ImplicitRegistration policySet
                                 ? policySet.Name
                                 : null;

                        if (null == name) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                var registration = (ExplicitRegistration)registry.Entries[index].Value;
                                value = (TElement)resolve(typeof(TElement), name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetEntries(hashGeneric, typeDefinition, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var key = registry.Entries[index].Value is ImplicitRegistration policySet
                                ? policySet.Name
                                : null;

                        if (null == key) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                int hash = NamedType.GetHashCode(typeof(TElement), key);
                                var registration = container.GetOrAdd(hash, typeof(TElement), key, registry.Entries[index].Value);
                                value = (TElement)resolve(typeof(TElement), key, registration);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidOperationException ex) when (ex.InnerException is InvalidRegistrationException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ComplexArray<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve, Type type)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hashCode = type.GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes and iterate over them
                var length = container._metadata.GetEntries(hashCode, type, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Value is ImplicitRegistration policySet
                                 ? policySet.Name
                                 : null;

                        if (null == name) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                int hash = NamedType.GetHashCode(typeof(TElement), name);
                                var registration = container.GetOrAdd(hash, typeof(TElement), name, registry.Entries[index].Value);
                                value = (TElement)resolve(typeof(TElement), name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ComplexArray<TElement>(Func<Type, string?, ImplicitRegistration, object?> resolve,
                                                              Type type, Type typeDefinition)
        {
            TElement value;
            var set = new QuickSet<Type>();
            int hashCode = type.GetHashCode();
            int hashGeneric = typeDefinition.GetHashCode();

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata || null == container._registry) continue;

                // Hold on to registries
                var registry = container._registry;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetEntries(hashCode, type, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Value is ImplicitRegistration policySet
                                 ? policySet.Name
                                 : null;

                        if (null == name) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                int hash = NamedType.GetHashCode(typeof(TElement), name);
                                var registration = container.GetOrAdd(hash, typeof(TElement), name, registry.Entries[index].Value);
                                value = (TElement)resolve(typeof(TElement), name, registration);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetEntries(hashGeneric, typeDefinition, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var key = registry.Entries[index].Value is ImplicitRegistration policySet
                                ? policySet.Name
                                : null;

                        if (null == key) continue;
                        if (set.Add(registry.Entries[index].HashCode, registry.Entries[index].Type))
                        {
                            try
                            {
                                int hash = NamedType.GetHashCode(typeof(TElement), key);
                                var registration = container.GetOrAdd(hash, typeof(TElement), key, registry.Entries[index].Value);
                                value = (TElement)resolve(typeof(TElement), key, registration);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidOperationException ex) when (ex.InnerException is InvalidRegistrationException)
                            {
                                continue;
                            }

                            yield return value;
                        }
                    }
                }
            }


        }

        #endregion

        #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        #pragma warning restore CS8603 // Possible null reference return.

        #region Resolve Delegate Factories

        private static ResolveDelegate<BuilderContext> OptimizingFactory(ref BuilderContext context)
        {
            throw new NotImplementedException();
            //var counter = 3;
            //var type = context.Type;
            //var registration = context.Registration;
            //ResolveDelegate<BuilderContext>? seed = null;
            //var chain = ((UnityContainer) context.Container)._processorsChain;

            //// Generate build chain
            //foreach (var processor in chain) seed = processor.GetResolver(type, registration, seed);

            //// Return delegate
            //return (ref BuilderContext c) => 
            //{
            //    // Check if optimization is required
            //    if (0 == Interlocked.Decrement(ref counter))
            //    {
            //        Task.Factory.StartNew(() => {

            //            // Compile build plan on worker thread
            //            var expressions = new List<Expression>();
            //            foreach (var processor in chain)
            //            {
            //                foreach (var step in processor.GetExpressions(type, registration))
            //                    expressions.Add(step);
            //            }

            //            expressions.Add(BuilderContextExpression.Existing);

            //            var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(
            //                Expression.Block(expressions), BuilderContextExpression.Context);

            //            // Replace this build plan with compiled
            //            registration.Set(typeof(ResolveDelegate<BuilderContext>), lambda.Compile());
            //        });
            //    }

            //    return seed?.Invoke(ref c);
            //};
        }

        internal ResolveDelegate<BuilderContext> CompilingFactory(ref BuilderContext context)
        {
            throw new NotImplementedException();
            //var expressions = new List<Expression>();
            //var type = context.Type;
            //var registration = context.Registration;

            //foreach (var processor in _processorsChain)
            //{
            //    foreach (var step in processor.GetExpressions(type, registration))
            //        expressions.Add(step);
            //}

            //expressions.Add(BuilderContextExpression.Existing);

            //var lambda = Expression.Lambda<ResolveDelegate<BuilderContext>>(
            //    Expression.Block(expressions), BuilderContextExpression.Context);

            //return lambda.Compile();
        }

        internal ResolveDelegate<BuilderContext> ResolvingFactory(ref BuilderContext context)
        {
            throw new NotImplementedException();
            //ResolveDelegate<BuilderContext>? seed = null;
            //var type = context.Type;
            //var registration = context.Registration;

            //foreach (var processor in _processorsChain)
            //    seed = processor.GetResolver(type, registration, seed);

            //return seed ?? ((ref BuilderContext c) => null);
        }

        #endregion


        #region Build Plans

        private ResolveDelegate<BuilderContext> ExecutePlan { get; set; } =
            (ref BuilderContext context) =>
            {
                var i = -1;
                BuilderStrategy[] chain = ((ImplicitRegistration)context.Registration).BuildChain;
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

                    throw new ResolutionFailedException(context.RegistrationType, context.Name,
                        "For more information add Diagnostic extension: Container.AddExtension(new Diagnostic())", ex);
                }

                return context.Existing;
            };

        private object? ExecuteValidatingPlan(ref BuilderContext context)
        {
            var i = -1;
            BuilderStrategy[] chain = context.Registration.BuildChain ?? _strategiesChain;

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

                var builder = new StringBuilder();
                builder.AppendLine(ex.Message);
                builder.AppendLine("_____________________________________________________");
                builder.AppendLine("Exception occurred while:");
                builder.AppendLine();

                var indent = 0;
                foreach (DictionaryEntry item in ex.Data)
                {
                    for (var c = 0; c < indent; c++) builder.Append(" ");
                    builder.AppendLine(CreateErrorMessage(item.Value));
                    indent += 1;
                }

                var message = builder.ToString();

                throw new ResolutionFailedException( context.RegistrationType, context.Name, message, ex);
            }

            return context.Existing;


            string CreateErrorMessage(object value)
            {
                switch (value)
                {
                    case ParameterInfo parameter:
                        return $" for parameter:  '{parameter.Name}'";

                    case ConstructorInfo constructor:
                        var ctorSignature = string.Join(", ", constructor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"on constructor:  {constructor.DeclaringType.Name}({ctorSignature})";

                    case MethodInfo method:
                        var methodSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        return $"     on method:  {method.Name}({methodSignature})";

                    case PropertyInfo property:
                        return $"  for property:  '{property.Name}'";

                    case FieldInfo field:
                        return $"     for field:  '{field.Name}'";

                    case Type type:
                        return $"·resolving type:  '{type.Name}'";

                    case Tuple<Type, string> tuple:
                        return $"•resolving type:  '{tuple.Item1.Name}' registered with name: '{tuple.Item2}'";

                    case Tuple<Type, Type> tuple:
                        return $"•resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}'";

                    case Tuple<Type, Type, string> tuple:
                        return $"•resolving type:  '{tuple.Item1?.Name}' mapped to '{tuple.Item2?.Name}' and registered with name: '{tuple.Item3}'";
                }

                return value.ToString();
            }
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

        internal static object? ContextValidatingExecutePlan(BuilderStrategy[] chain, ref BuilderContext context)
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
            object? GetPerResolveValue(IntPtr parent, Type registrationType, string? name)
            {
                if (IntPtr.Zero == parent) return null;

                unsafe
                {
                    var parentRef = Unsafe.AsRef<BuilderContext>(parent.ToPointer());
                    if (registrationType != parentRef.RegistrationType || name != parentRef.Name)
                        return GetPerResolveValue(parentRef.Parent, registrationType, name);

                    var lifetimeManager = (LifetimeManager?)parentRef.Get(typeof(LifetimeManager));
                    var result = lifetimeManager?.GetValue();
                    if (LifetimeManager.NoValue != result) return result;

                    throw new InvalidOperationException($"Circular reference for Type: {parentRef.Type}, Name: {parentRef.Name}",
                            new CircularDependencyException());
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
                        throw new InvalidOperationException($"Circular reference for Type: {thisContext.Type}, Name: {thisContext.Name}",
                            new CircularDependencyException());

                    parent = parentRef.Parent;
                }

                var context = new BuilderContext
                {
                    Lifetime = thisContext.Lifetime,
                    Registration = thisContext.Registration,
                    RegistrationType = thisContext.Type,
                    Name = thisContext.Name,
                    Type = thisContext.Type,
                    Compose = thisContext.Compose,
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
