using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Builder
{
    /// <summary>
    /// An implementation of <see cref="ISelect{ConstructorInfo}"/> that is
    /// aware of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityConstructorSelector : MemberSelectorBase<ConstructorInfo, object[]>,
                                                   ISelect<ConstructorInfo>
    {
        #region Fields

        private static readonly ConstructorLengthComparer ConstructorComparer = new ConstructorLengthComparer();

        #endregion


        #region Constructors

        public DefaultUnityConstructorSelector()
            : base(new (Type type, Converter<ConstructorInfo, object> factory)[]
                { (typeof(InjectionConstructorAttribute), info => info) })
        {
            
        }

        #endregion


        #region IConstructorSelectorPolicy

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        public IEnumerable<object> Select(ref BuilderContext context)
        {
            var value = OnSelect(ref context).FirstOrDefault();
            return null == value 
                ? new []{ GetDefaultMember(context.Type) }
                : new[] { value };
        }


        #endregion


        #region Overrides

        protected override ConstructorInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                       .ToArray();
#endif
        }

        protected object GetDefaultMember(Type type)
        {
            ConstructorInfo[] constructors = DeclaredMembers(type);
            Array.Sort(constructors, ConstructorComparer);

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
                                type.GetTypeInfo().Name,
                                paramLength));
                    }
                    return constructors[0];
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
