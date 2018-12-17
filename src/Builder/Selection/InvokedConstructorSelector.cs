using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Builder
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that is
    /// aware of the build keys used by the Unity container.
    /// </summary>
    public class InvokedConstructorSelector : MemberSelectorPolicy<ConstructorInfo, object[]>, 
                                                         IConstructorSelectorPolicy
    {
        #region Fields

        private static readonly ConstructorLengthComparer ConstructorComparer = new ConstructorLengthComparer();

        #endregion


        #region IConstructorSelectorPolicy

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        public object SelectConstructor<TContext>(ref TContext context) 
            where TContext : IBuilderContext 
            => Select(ref context).FirstOrDefault();
        

        #endregion


        #region Overrides

        protected override IEnumerable<object> GetAttributedMembers(Type type)
        {
            var constructors = type.GetTypeInfo()
                                   .DeclaredConstructors
                                   .Where(c => c.IsStatic == false && c.IsPublic &&
                                               c.IsDefined(typeof(InjectionConstructorAttribute),
                                                   true))
                                   .ToArray();
            switch (constructors.Length)
            {
                case 0:
                    yield break;

                case 1:
                    yield return constructors[0];
                    break;

                default:
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Constants.MultipleInjectionConstructors,
                            type.GetTypeInfo().Name));
            }
        }

        protected override IEnumerable<object> GetDefaultMember(Type type)
        {
            ConstructorInfo[] constructors = type.GetTypeInfo()
                                                 .DeclaredConstructors
                                                 .Where(c => c.IsStatic == false && c.IsPublic)
                                                 .ToArray();

            Array.Sort(constructors, ConstructorComparer);

            switch (constructors.Length)
            {
                case 0:
                    yield break;

                case 1:
                    yield return constructors[0];
                    break;

                default:
                    var paramLength = constructors[0].GetParameters().Length;
                    if (constructors[1].GetParameters().Length == paramLength)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Constants.AmbiguousInjectionConstructor,
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    yield return constructors[0];
                    break;
            }
        }

        #endregion


        #region Nested Types

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            public int Compare(ConstructorInfo x, ConstructorInfo y) => y?.GetParameters().Length ?? 0 - x?.GetParameters().Length ?? 0;
        }

        #endregion
    }
}
