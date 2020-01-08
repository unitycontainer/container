using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Exceptions;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
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
                    return IsRegistered(type, name) || CanResolve(type.GetElementType()!, name);
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


        #region Resolving Enumerable

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string?, object?> resolve, string? name)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(typeof(TElement));

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes and iterate over them
                var length = container._metadata.GetMeta(ref key, out int[]? data);
                if (null != data && null != registry)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var registrationName = registry.Entries[index].Key.Name;

                        if (!set.Add(registrationName)) continue;

                        try
                        {
                            value = resolve(typeof(TElement), registrationName);
                        }
                        catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                        {
                            continue;
                        }

                        yield return null == value ? default : (TElement)value;
                    }
                }
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    value = resolve(typeof(TElement), name);
                }
                catch
                {
                    yield break;
                }

#pragma warning disable CS8601 // Possible null reference assignment.
                yield return (TElement)value;
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        internal IEnumerable<TElement> ResolveEnumerable<TElement>(Func<Type, string?, object?> resolve,
                                                                   Type typeDefinition, string? name)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(typeof(TElement));
            var keyGeneric = new HashKey(typeDefinition);

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetMeta(ref key, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var registrationName = registry.Entries[index].Key.Name;

                        if (!set.Add(registrationName)) continue;

                        try
                        {
                            value = resolve(typeof(TElement), registrationName);
                        }
                        catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                        {
                            continue;
                        }

                        yield return null == value ? default : (TElement)value;
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetMeta(ref keyGeneric, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var registration = (ExplicitRegistration)registry.Entries[index].Policies;

                        if (set.Add(registration.Name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), registration.Name);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidRegistrationException)   { continue; }

                            yield return null == value ? default : (TElement)value;
                        }
                    }
                }
            }

            // If nothing registered attempt to resolve the type
            if (0 == set.Count)
            {
                try
                {
                    value = resolve(typeof(TElement), name);
                }
                catch
                {
                    yield break;
                }

                yield return null == value ? default : (TElement)value;
            }
        }

        #endregion


        #region Resolving Array

        internal Type GetTargetType(Type argType)
        {
            Type? next;
            for (Type? type = argType; null != type; type = next)
            {
                var info = type.GetTypeInfo();
                if (info.IsGenericType)
                {
                    if (IsRegistered(type)) return type!;

                    var definition = info.GetGenericTypeDefinition();
                    if (IsRegistered(definition)) return definition;

                    next = info.GenericTypeArguments[0]!;
                    if (IsRegistered(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType()!;
                    if (IsRegistered(next)) return next;
                }
                else
                {
                    return type!;
                }
            }

            return argType;
        }

        internal IEnumerable<TElement> ResolveArray<TElement>(Func<Type, string?, object?> resolve, Type type)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(type);

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes and iterate over them
                var length = container._metadata.GetMeta(ref key, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

#pragma warning disable CS8601 // Possible null reference assignment.
                            yield return (TElement)value;
#pragma warning restore CS8601 // Possible null reference assignment.
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ResolveArray<TElement>(Func<Type, string?, object?> resolve,
                                                              Type type, Type typeDefinition)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(typeof(TElement));
            var keyType = new HashKey(type);
            var keyGeneric = new HashKey(typeDefinition);

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetMeta(ref keyType, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

#pragma warning disable CS8601 // Possible null reference assignment.
                            yield return (TElement)value;
#pragma warning restore CS8601 // Possible null reference assignment.
                        }
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetMeta(ref keyGeneric, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidRegistrationException)   { continue; }

#pragma warning disable CS8601 // Possible null reference assignment.
                            yield return (TElement)value;
#pragma warning restore CS8601 // Possible null reference assignment.
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ComplexArray<TElement>(Func<Type, string?, object?> resolve, Type type)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(typeof(TElement));
            var typeKey = new HashKey(type);

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes and iterate over them
                var length = container._metadata.GetMeta(ref typeKey, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

#pragma warning disable CS8601 // Possible null reference assignment.
                            yield return (TElement)value;
#pragma warning restore CS8601 // Possible null reference assignment.
                        }
                    }
                }
            }
        }

        internal IEnumerable<TElement> ComplexArray<TElement>(Func<Type, string?, object?> resolve,
                                                              Type type, Type typeDefinition)
        {
            object? value;
            var set = new QuickSet();
            var key = new HashKey(type);
            var keyGeneric = new HashKey(typeDefinition);

            // Iterate over hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no data
                if (null == container._metadata) continue;

                // Hold on to registries
                Debug.Assert(null != container._registry);
                var registry = container._registry!;

                // Get indexes for bound types and iterate over them
                var length = container._metadata.GetMeta(ref key, out int[]? data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                            {
                                continue;
                            }

                            yield return null == value ? default : (TElement)value;
                        }
                    }
                }

                // Get indexes for unbound types and iterate over them
                length = container._metadata.GetMeta(ref keyGeneric, out data);
                if (null != data)
                {
                    for (var i = 1; i < length; i++)
                    {
                        var index = data[i];
                        var name = registry.Entries[index].Key.Name;

                        if (null != name && set.Add(name))
                        {
                            try
                            {
                                value = resolve(typeof(TElement), name);
                            }
                            catch (MakeGenericTypeFailedException) { continue; }
                            catch (InvalidRegistrationException)   { continue; }

                            yield return null == value ? default : (TElement)value;
                        }
                    }
                }
            }


        }

        #endregion


        #region Resolve Delegate Factories

        private static ResolveDelegate<PipelineContext> OptimizingFactory(ref PipelineContext context)
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

        internal ResolveDelegate<PipelineContext> CompilingFactory(ref PipelineContext context)
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

        internal ResolveDelegate<PipelineContext> ResolvingFactory(ref PipelineContext context)
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
    }
}
