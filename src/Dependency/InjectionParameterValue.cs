using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Build;
using Unity.Delegates;
using Unity.Factory;
using Unity.Policy;

namespace Unity.Injection
{
    /// <summary>
    /// Base type for objects that are used to configure parameters for
    /// constructor or method injection, or for getting the value to
    /// be injected into a property.
    /// </summary>
    public abstract class InjectionParameterValue : IResolverFactory
    {
        #region Constructors

        protected InjectionParameterValue(object value = null)
        {
            Value = value;
        }

        #endregion

        public object Value { get; }


        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public abstract string ParameterTypeName
        {
            get;
        }

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        public abstract bool MatchesType(Type t);


        public virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IBuildContext
        {
            return Value is IResolverFactory factory 
                ? factory.GetResolver<TContext>(type) 
                : (ref TContext c) => Value;
        }

        protected static object ResolveValue<TContext>(ref TContext context, object value)
            where TContext : IBuildContext
        {
            switch (value)
            {
                case ResolveDelegate<TContext> resolver:
                    return resolver(ref context);

                case IResolverPolicy policy:
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
}
