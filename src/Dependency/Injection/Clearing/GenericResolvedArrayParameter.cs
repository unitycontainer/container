using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    public class GenericResolvedArrayParameter : GenericBase, IResolverFactory
    {
        private readonly object[] _values;

        private static readonly MethodInfo ResolverMethod =
            typeof(GenericResolvedArrayParameter).GetTypeInfo().GetDeclaredMethod(nameof(DoResolve));

        private delegate object Resolver<TContext>(ref TContext context, object[] values) 
            where TContext : IResolveContext;

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

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => base.ParameterTypeName + "[]";



        #region  GenericBase

        public override bool Equals(Type type)
        {
            var t = type ?? throw new ArgumentNullException(nameof(type));

            if (!t.IsArray || t.GetArrayRank() != 1)
            {
                return false;
            }

            Type elementType = t.GetElementType();
            return elementType.GetTypeInfo().IsGenericParameter && elementType.GetTypeInfo().Name == base.ParameterTypeName;
        }

        public override ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
        {
            var typeToResolve = info.ParameterType.GetElementType();
            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), typeToResolve)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory factory:
                        return factory.GetResolver<TContext>(typeToResolve);

                    case Type _ when typeof(Type) != typeToResolve:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(typeToResolve, null));

                    default:
                        return value;
                }

            }).ToArray();

            return (ref TContext context) => resolverMethod.Invoke(ref context, values);
        }

        #endregion


        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.NotAGenericType,
                        typeToBuild.GetTypeInfo().Name,
                        base.ParameterTypeName));
            }
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type genericParam in typeToBuild.GetGenericTypeDefinition().GetTypeInfo().GenericTypeParameters)
            {
                if (genericParam.GetTypeInfo().Name == base.ParameterTypeName)
                {
                    return;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Constants.NoMatchingGenericArgument,
                    typeToBuild.GetTypeInfo().Name,
                    base.ParameterTypeName));
        }


        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            Type elementType;
            Type typeToResolve;
            if (type.GetTypeInfo().IsGenericType)
            {
                GuardTypeToBuildHasMatchingGenericParameter(type);
                typeToResolve = type.GetNamedGenericParameter(base.ParameterTypeName);
                elementType = type;
            }
            else
            {
                typeToResolve = type.GetElementType();
                elementType = typeToResolve;
            }


            var resolverMethod = (Resolver<TContext>)ResolverMethod.MakeGenericMethod(typeof(TContext), typeToResolve)
                                                                   .CreateDelegate(typeof(Resolver<TContext>));
            var values = _values.Select(value =>
            {
                switch (value)
                {
                    case IResolverFactory factory:
                        return factory.GetResolver<TContext>(elementType);

                    case Type _ when typeof(Type) != typeToResolve:
                        return (ResolveDelegate<TContext>)((ref TContext context) => context.Resolve(typeToResolve, null));

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
    }
}
