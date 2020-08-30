using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    [DebuggerDisplay("GenericResolvedArrayParameter: Type={ParameterTypeName}")]
    public class GenericResolvedArrayParameter : GenericParameterBase
    {
        #region Fields

        private readonly object[] _values;

        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve))!;

        private delegate object Resolver<TContext>(ref TContext context, object[] values) 
            where TContext : IResolveContext;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericResolvedArrayParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
            : base(genericParameterName)
        {
            _values = elementValues;
        }

        #endregion


        #region IMatch

        public override MatchRank MatchTo(Type type)
        {
            if (!type.IsArray || type.GetArrayRank() != 1)
                return MatchRank.NoMatch;

            Type elementType = type.GetElementType()!;
            return elementType.IsGenericParameter && elementType.Name == base.ParameterTypeName
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion


        #region Overrides

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => base.ParameterTypeName + "[]";

        protected override ResolveDelegate<TContext> GetResolver<TContext>(Type type, string? name)
        {
            if (!type.IsArray) throw new InvalidOperationException($"Type {type} is not an Array. {GetType().Name} can only resolve array types.");

            Type elementType = type.GetElementType()!;
            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), elementType)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory<Type> factory:
                        return factory.GetResolver<TContext>(elementType);

                    case Type _ when typeof(Type) != elementType:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(elementType, null));

                    default:
                        return value;
                }

            }).ToArray();

            return (ref TContext context) => resolverMethod.Invoke(ref context, values);
        }

        public override string ToString()
        {
            return $"GenericResolvedArrayParameter: Type={ParameterTypeName}";
        }

        #endregion


        #region Implementation

        public static object DoResolve<TContext, TElement>(ref TContext context, object[] values)
            where TContext : IResolveContext
            where TElement : class
        {
            var result = new TElement?[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = (TElement?)ResolveValue(ref context, values[i]);
            }

            return result;

            // Interpret factories
            object? ResolveValue(ref TContext c, object value)
            {
                switch (value)
                {
                    case ResolveDelegate<TContext> resolver:
                        return resolver(ref c);

                    case IResolve policy:
                        return policy.Resolve(ref c);
                }

                return value;
            }
        }

        #endregion
    }
}
