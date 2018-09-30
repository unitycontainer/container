using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Build;
using Unity.Builder.Selection;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan.Selection
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IConstructorSelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    public abstract class ConstructorSelectorPolicyBase<TInjectionConstructorMarkerAttribute> : IConstructorSelectorPolicy
        where TInjectionConstructorMarkerAttribute : Attribute
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        public object SelectConstructor<TContext>(ref TContext context)
            where TContext : IBuildContext
        {
            Type typeToConstruct = context.Type;
            ConstructorInfo ctor = FindInjectionConstructor(typeToConstruct) ?? FindLongestConstructor(typeToConstruct);
            if (ctor != null)
            {
                return CreateSelectedConstructor(ctor);
            }
            return null;
        }

        private SelectedConstructor CreateSelectedConstructor(ConstructorInfo ctor)
        {
            var result = new SelectedConstructor(ctor);

            foreach (ParameterInfo param in ctor.GetParameters())
            {
                result.AddParameterResolver(CreateResolver(param));
            }

            return result;
        }

        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected abstract IResolverPolicy CreateResolver(ParameterInfo parameter);

        private static ConstructorInfo FindInjectionConstructor(Type typeToConstruct)
        {
            var constructors = typeToConstruct.GetTypeInfo()
                                              .DeclaredConstructors
                                              .Where(c => c.IsStatic == false && c.IsPublic &&
                                                          c.IsDefined(typeof(TInjectionConstructorMarkerAttribute),
                                                                      true))
                                              .ToArray();
            switch (constructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return constructors[0];

                default:
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Constants.MultipleInjectionConstructors,
                            typeToConstruct.GetTypeInfo().Name));
            }
        }

        private static ConstructorInfo FindLongestConstructor(Type typeToConstruct)
        {
            ConstructorInfo[] constructors = typeToConstruct.GetTypeInfo()
                                                            .DeclaredConstructors
                                                            .Where(c => c.IsStatic == false && c.IsPublic)
                                                            .ToArray();
            Array.Sort(constructors, new ConstructorLengthComparer());

            switch (constructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return constructors[0];

                default:
                    int paramLength = constructors[0].GetParameters().Length;
                    if (constructors[1].GetParameters().Length == paramLength)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Constants.AmbiguousInjectionConstructor,
                                typeToConstruct.GetTypeInfo().Name,
                                paramLength));
                    }
                    return constructors[0];
            }
        }

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="y">The second object to compare.</param>
            /// <param name="x">The first object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero is less than y. Zero equals y. Greater than zero is greater than y.
            /// </returns>
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                return (y ?? throw new ArgumentNullException(nameof(y))).GetParameters().Length - (x ?? throw new ArgumentNullException(nameof(x))).GetParameters().Length;
            }
        }
    }
}
