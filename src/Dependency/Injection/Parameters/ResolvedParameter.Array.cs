using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    [DebuggerDisplay("ResolvedArrayParameter: Type={ParameterType.Name}")]
    public class ResolvedArrayParameter : ParameterBase,
                                          IResolverFactory<Type>,
                                          IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly object[] _values;
        private readonly Type _elementType;

        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo()
                                                 .GetDeclaredMethod(nameof(GenericResolvedArrayParameter.DoResolve))!;

        private delegate object Resolver<TContext>(ref TContext context, object[] values)
            where TContext : IResolveContext;

        #endregion


        #region Constructors

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type and collection of element values.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public ResolvedArrayParameter(Type elementType, params object[] elementValues)
            : this((elementType ?? throw new ArgumentNullException(nameof(elementType))).MakeArrayType(), 
                    elementType, elementValues)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types and collection of element values.
        /// </summary>
        /// <param name="contractType">The contract type for the array</param>
        /// <param name="elementType">The type of elements to resolve</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type contractType, Type elementType, params object[] elementValues)
            : base(contractType, false)
        {
            _elementType = elementType;
            _values = elementValues;

            // Exit if no data
            if (null == elementValues || 0 == elementValues.Length) 
                return;

            // Verify array elements
            foreach (var pv in elementValues)
            {
                if ((pv is IMatch<Type> other && MatchRank.NoMatch != other.Match(elementType)) ||
                    (pv is Type type && type == _elementType) || _elementType.IsAssignableFrom(pv?.GetType()!))
                    continue;

                throw new InvalidOperationException(
                    $"The type {pv?.GetType()} cannot be assigned to variables of type {elementType}.");
            }
        }

        #endregion


        #region IResolverFactory

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), _elementType)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory<Type> factory:
                        return factory.GetResolver<TContext>(_elementType);

                    case Type _ when typeof(Type) != _elementType:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(_elementType, null));

                    default:
                        return value;
                }

            }).ToArray();

            return (ref TContext context) => resolverMethod.Invoke(ref context, values);
        }

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext
        {
            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), _elementType)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory<Type> factory:
                        return factory.GetResolver<TContext>(_elementType);

                    case Type _ when typeof(Type) != _elementType:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(_elementType, null));

                    default:
                        return value;
                }

            }).ToArray();

            return (ref TContext context) => resolverMethod.Invoke(ref context, values);
        }

        #endregion


        #region Overrides

        public override string ToString()
        {
            return $"ResolvedArrayParameter: Type={ParameterType!.Name}";
        }

        #endregion
    }

    /// <summary>
    /// A generic version of <see cref="ResolvedArrayParameter"/> for convenience
    /// when creating them by hand.
    /// </summary>
    /// <typeparam name="TElement">Type of the elements for the array of the parameter.</typeparam>
    public class ResolvedArrayParameter<TElement> : ResolvedArrayParameter
    {
        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter{TElement}"/> that
        /// resolves to the given element generic type with the given element values.
        /// </summary>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public ResolvedArrayParameter(params object[] elementValues)
            : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }
}
