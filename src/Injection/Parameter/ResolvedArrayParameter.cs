using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Build;
using Unity.Delegates;
using Unity.Factory;
using Unity.Injection;
using Unity.Policy;
using Unity.Utility;

namespace Unity
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    public class ResolvedArrayParameter : TypedInjectionValue
    {
        private readonly Type _elementType;
        private readonly object[] _values;
        private readonly List<InjectionParameterValue> _elementValues = new List<InjectionParameterValue>();
        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));
        private delegate object Resolver<TContext>(ref TContext context, object[] values)
            where TContext : IBuildContext;

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given element type and collection of element values.
        /// </summary>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(Type elementType, params object[] elementValues)
            : this(elementType.MakeArrayType(), elementType, elementValues)
        {
        }

        /// <summary>
        /// Construct a new <see cref="ResolvedArrayParameter"/> that
        /// resolves to the given array and element types and collection of element values.
        /// </summary>
        /// <param name="arrayParameterType">The type for the array of elements to resolve.</param>
        /// <param name="elementType">The type of elements to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType, params object[] elementValues)
            : base(arrayParameterType, null)
        {
            _elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            _values = elementValues;

            _elementValues.AddRange(ToParameters(elementValues ?? throw new ArgumentNullException(nameof(elementValues))));
            foreach (InjectionParameterValue pv in _elementValues)
            {
                if (!pv.MatchesType(elementType))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Constants.TypesAreNotAssignable,
                            elementType,
                            pv.ParameterTypeName));
                }
            }
        }

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            var elementType = !_elementType.IsArray ? _elementType
                : _elementType.GetArrayParameterType(type.GetTypeInfo().GenericTypeArguments);

            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), elementType)
                .CreateDelegate(typeof(Resolver<TContext>));

            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory factory:
                        return factory.GetResolver<TContext>(type);

                    case Type _ when typeof(Type) != elementType:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(elementType, null));

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
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public ResolvedArrayParameter(params object[] elementValues)
            : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }
}
