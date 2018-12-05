using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Build;
using Unity.Factory;
using Unity.Utility;

namespace Unity
{
    /// <summary>
    /// A <see cref="InjectionParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    public class GenericResolvedArrayParameter : InjectionParameterValue
    {
        private readonly object[] _values;
        private readonly string _genericParameterName;

        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));
        private delegate object Resolver<TContext>(ref TContext context, object[] values) 
            where TContext : IBuildContext;

        /// <summary>
        /// Create a new <see cref="GenericResolvedArrayParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
        {
            _genericParameterName = genericParameterName ?? throw new ArgumentNullException(nameof(genericParameterName));
            _values = elementValues;
        }

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => _genericParameterName + "[]";

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="type"/>,
        /// false if not.</returns>
        /// <remarks>A type is considered compatible if it is an array type of rank one
        /// and its element type is a generic type parameter with a name matching this generic
        /// parameter name configured for the receiver.</remarks>
        public override bool MatchesType(Type type)
        {
            var t = type ?? throw new ArgumentNullException(nameof(type));

            if (!t.IsArray || t.GetArrayRank() != 1)
            {
                return false;
            }

            Type elementType = t.GetElementType();
            return elementType.GetTypeInfo().IsGenericParameter && elementType.GetTypeInfo().Name == _genericParameterName;
        }


        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.NotAGenericType,
                        typeToBuild.GetTypeInfo().Name,
                        _genericParameterName));
            }
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type genericParam in typeToBuild.GetGenericTypeDefinition().GetTypeInfo().GenericTypeParameters)
            {
                if (genericParam.GetTypeInfo().Name == _genericParameterName)
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Constants.NoMatchingGenericArgument,
                    typeToBuild.GetTypeInfo().Name,
                    _genericParameterName));
        }

        public override BuildDelegate<TContext> GetResolver<TContext>(Type type)
        {
            GuardTypeToBuildIsGeneric(type);
            GuardTypeToBuildHasMatchingGenericParameter(type);

            var typeToResolve = type.GetNamedGenericParameter(_genericParameterName);
            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), typeToResolve)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory factory:
                        return factory.GetResolver<TContext>(type);

                    case Type _ when typeof(Type) != typeToResolve:
                        return (BuildDelegate<TContext>)((ref TContext context) => context.Resolve(typeToResolve, null));

                    default:
                        return value;
                }

            }).ToArray();

            return (ref TContext context) => resolverMethod.Invoke(ref context, values);
        }

        private static object DoResolve<TContext, TElement>(ref TContext context, object[] values)
            where TContext : IBuildContext
        {
            var result = new TElement[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = (TElement)ResolveValue(ref context, values[i]);
            }

            return result;
        }
    }
}
