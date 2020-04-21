using System;
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
    public class ResolvedArrayParameter : ParameterBase,
                                          IResolverFactory<Type>,
                                          IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly object[] _values;
        private readonly Type _elementType;

        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));

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
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType, params object[] elementValues)
            : base(arrayParameterType)
        {
            _elementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
            _values = elementValues;

            // Verify array elements
            foreach (var pv in elementValues)
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                var info = _elementType.GetTypeInfo();
                if ((pv is IEquatable<Type> equatable && equatable.Equals(elementType)) ||
                    (pv is Type type && type == _elementType) || info.IsAssignableFrom(pv?.GetType().GetTypeInfo()))
                    continue;
#else
                if ((pv is IEquatable<Type> equatable && equatable.Equals(elementType)) ||
                    (pv is Type type && type == _elementType) || _elementType.IsAssignableFrom(pv?.GetType()))
                    continue;
#endif
                throw new InvalidOperationException(
                    $"The type {pv?.GetType()} cannot be assigned to variables of type {elementType}.");
            }
        }

        #endregion


        #region IResolverFactory

        public ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            var elementType = !_elementType.IsArray
                            ? _elementType
                            : _elementType.GetArrayParameterType(type.GetTypeInfo()
                                                                     .GenericTypeArguments);

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

        public ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext
        {
            var elementType = info.ParameterType.IsArray ? info.ParameterType.GetElementType() : _elementType;
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

        #endregion


        #region Implementation

        private static object DoResolve<TContext, TElement>(ref TContext context, object[] values)
            where TContext : IResolveContext
        {
            var result = new TElement[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                result[i] = (TElement)ResolveValue(ref context, values[i]);
            }

            return result;


            object ResolveValue(ref TContext c, object value)
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
