using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A class that stores a type, and generates a 
    /// resolver object that resolves all the named instances or the
    /// type registered in a container.
    /// </summary>
    public class ResolvedArrayParameter : TypedInjectionValue, IResolverFactory
    {
        private readonly Type _elementType;
        private readonly object[] _values;
        private readonly List<InjectionParameterValue> _elementValues = new List<InjectionParameterValue>();
        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));
        private delegate object Resolver<TContext>(ref TContext context, object[] values)
            where TContext : IResolveContext;

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
            : base(arrayParameterType)
        {
            _elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            _values = elementValues;

            _elementValues.AddRange(ToParameters(elementValues ?? throw new ArgumentNullException(nameof(elementValues))));
            foreach (InjectionParameterValue pv in _elementValues)
            {
                if (pv is IEquatable<Type> equatable && !equatable.Equals(elementType))
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

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
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
            where TContext : IResolveContext
        {
            var result = new TElement[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = (TElement)ResolveValue(ref context, values[i]);
            }

            return result;
        }

        protected static object ResolveValue<TContext>(ref TContext context, object value)
            where TContext : IResolveContext
        {
            switch (value)
            {
                case ResolveDelegate<TContext> resolver:
                    return resolver(ref context);

                case IResolve policy:
                    return policy.Resolve(ref context);
            }

            return value;
        }

        /// <summary>
        /// Convert the given set of arbitrary values to a sequence of InjectionParameterValue
        /// objects. The rules are: If it's already an InjectionParameterValue, return it. If
        /// it's a Type, return a ResolvedParameter object for that type. Otherwise return
        /// an InjectionParameter object for that value.
        /// </summary>
        /// <param name="values">The values to build the sequence from.</param>
        /// <returns>The resulting converted sequence.</returns>
        public static IEnumerable<InjectionParameterValue> ToParameters(params object[] values)
        {
            foreach (object value in values)
            {
                yield return ToParameter(value);
            }
        }

        /// <summary>
        /// Convert an arbitrary value to an InjectionParameterValue object. The rules are: 
        /// If it's already an InjectionParameterValue, return it. If it's a Type, return a
        /// ResolvedParameter object for that type. Otherwise return an InjectionParameter
        /// object for that value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The resulting <see cref="InjectionParameterValue"/>.</returns>
        public static InjectionParameterValue ToParameter(object value)
        {
            switch (value)
            {
                case InjectionParameterValue parameterValue:
                    return parameterValue;

                case Type typeValue:
                    return new ResolvedParameter(typeValue);
            }

            return new InjectionParameter(value);
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
