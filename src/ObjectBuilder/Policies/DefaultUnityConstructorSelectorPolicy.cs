using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Registration;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that is
    /// aware of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityConstructorSelectorPolicy : IConstructorSelectorPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        public object SelectConstructor<TContext>(ref TContext context)
            where TContext : IBuilderContext
        {
            // TODO: Requires allocation optimization
            return FindInjectionConstructor(context.Type, ((InternalRegistration)context.Registration).InjectionMembers) ?? 
                   FindAttributedConstructor(context.Type) ??
                   FindLongestConstructor(context.Type);
        }

        private static object FindInjectionConstructor(Type typeToConstruct, IInjectionMember[] injectionMembers)
        {
            if (null == injectionMembers) return null;

            foreach (var member in injectionMembers)
            {
                if (member is InjectionConstructor)
                    return member;
            }

            return null;
        }

        private static object FindAttributedConstructor(Type typeToConstruct)
        {
            var constructors = typeToConstruct.GetTypeInfo()
                                              .DeclaredConstructors
                                              .Where(c => c.IsStatic == false && c.IsPublic &&
                                                          c.IsDefined(typeof(InjectionConstructorAttribute),
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

        private static object FindLongestConstructor(Type typeToConstruct)
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
                    var paramLength = constructors[0].GetParameters().Length;
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
            /// InjectionParameterValue Condition Less than zero is less than y. Zero equals y. Greater than zero is greater than y.
            /// </returns>
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                return (y ?? throw new ArgumentNullException(nameof(y))).GetParameters().Length - (x ?? throw new ArgumentNullException(nameof(x))).GetParameters().Length;
            }
        }
    }
}
