using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Container;
using Unity.Exceptions;
using Unity.ObjectBuilder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    /// <inheritdoc />
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer
    {
        #region Fields

        



        #endregion



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
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="typeToBuild"/>).</returns>
        public object BuildUp(Type typeToBuild, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            var type = typeToBuild ?? throw new ArgumentNullException(nameof(typeToBuild));
            if (null != existing) InstanceIsAssignable(type, existing, nameof(existing));

            var key = new NamedTypeBuildKey(type, name);
            IBuilderContext context = null;

            try
            {
                context = new BuilderContext(this, new StrategyChain(_strategies ),
                    _lifetimeContainer,
                    _context,
                    key,
                    existing);

                if (null != resolverOverrides && 0 != resolverOverrides.Length)
                    context.AddResolverOverrides(resolverOverrides);

                if (key.Type.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                            Constants.CannotResolveOpenGenericType,
                            key.Type.FullName), nameof(key.Type));
                }

                context.Strategies.BuildUp(context);
                return context.Existing;
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(key.Type, key.Name, ex, context);
            }
        }


        #endregion


        #region Select Type

        private StagedStrategyChain<Func<UnityContainer, Type, string, INamedType>, TypeSelectStage> GetTypeSelectStage()
        {
            return new StagedStrategyChain<Func<UnityContainer, Type, string, INamedType>, TypeSelectStage>
            {
                { GetRegisteredType , TypeSelectStage.Registration },
                { GetArrayType, TypeSelectStage.Array },
                { GetGenericType, TypeSelectStage.Generic },
                { GerPocoType, TypeSelectStage.Poco },
                { GetUnknownType, TypeSelectStage.Exception }
            };
        }

        private INamedType GetRegisteredType(UnityContainer context, Type type, string name)
        {
            for (var container = this; null != container; container = container._parent)
            {
                var data = container[type, name];
                if (null == data) continue;

                return (INamedType)data;
            }

            return null;
        }

        private INamedType GetArrayType(UnityContainer context, Type type, string name)
        {
            throw new NotImplementedException();
        }

        private INamedType GetGenericType(UnityContainer context, Type type, string name)
        {
            throw new NotImplementedException();
        }

        private INamedType GerPocoType(UnityContainer context, Type type, string name)
        {
            throw new NotImplementedException();
        }

        private INamedType GetUnknownType(UnityContainer context, Type type, string name)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
