using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Exceptions;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    /// <inheritdoc />
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
        #region Getting objects

        /// <summary>
        /// GetOrDefault an instance of the requested type with the given name typeFrom the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public object Resolve(Type type, string name, params ResolverOverride[] resolverOverrides)
        {
            return BuildUp(type, null, name, resolverOverrides);
        }

        #endregion


        #region BuildUp existing object

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don'type control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para></remarks>
        /// <param name="typeToBuild"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="nameToBuild">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="typeToBuild"/>).</returns>
        public object BuildUp(Type typeToBuild, object existing, string nameToBuild, params ResolverOverride[] resolverOverrides)
        {
            // Verify arguments
            var name = string.IsNullOrEmpty(nameToBuild) ? null : nameToBuild;
            var type = typeToBuild ?? throw new ArgumentNullException(nameof(typeToBuild));
            if (null != existing) InstanceIsAssignable(type, existing, nameof(existing));

            IBuilderContext context = null;

            try
            {
                context = new BuilderContext(_context, _lifetimeContainer, _strategies, _context,
                                             Registration(type, name, true),
                                             existing, resolverOverrides);

                if (type.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                              Constants.CannotResolveOpenGenericType,
                                                              type.FullName), nameof(typeToBuild));
                }

                context.Strategies.BuildUp(context);
                return context.Existing;
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(type, name, ex, context);
            }
        }


        #endregion


        #region Resolving Enumerables

        private static void ResolveArray<T>(IBuilderContext context)
        {
            if (null != context.Existing) return;

            var container = (UnityContainer)context.Container;
            context.Existing = container.GetRegisteredNames(container, typeof(T))
                .Where(registration => null != registration)
                .Select(registration => context.NewBuildUp(typeof(T), registration))
                .Cast<T>()
                .ToArray();

            context.BuildComplete = true;
            context.SetPerBuildSingleton();
        }

        private static void ResolveEnumerable<T>(IBuilderContext context)
        {
            if (null != context.Existing) return;

            var container = (UnityContainer)context.Container;
            context.Existing = container.GetRegisteredNames(container, typeof(T))
                .Select(registration => context.NewBuildUp(typeof(T), registration))
                .Cast<T>()
                .ToArray();

            context.BuildComplete = true;
            context.SetPerBuildSingleton();
        }

        #endregion


        #region Implementation


        /// <summary>
        /// Retrieves registration for requested named type
        /// </summary>
        /// <param name="type">Registration type</param>
        /// <param name="name">Registration name</param>
        /// <param name="create">Instruncts container if it should create registration if not found</param>
        /// <returns>Registration for requested named type or null if named type is not registered and 
        /// <see cref="create"/> is false</returns>
        public INamedType Registration(Type type, string name, bool create = false)
        {
            for (var container = this; null != container; container = container._parent)
            {
                IPolicySet data;
                if (null == (data = container[type, name])) continue;

                return (INamedType)data;
            }

            if (!create) return null;

            var collisions = 0;
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return (INamedType)existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name);
                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = registration };
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;

                return (INamedType)registration;
            }
        }


        #endregion
    }
}
