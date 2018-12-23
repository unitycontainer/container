using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Builder
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that is aware
    /// of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityMethodsSelector : MemberSelectorBase<MethodInfo, object[]>, 
                                          IMethodSelectorPolicy
    {
        #region Constructors

        public DefaultUnityMethodsSelector()
            : base(new (Type type, Converter<MethodInfo, object> factory)[]
                { (typeof(InjectionMethodAttribute), info => info) })
        {
        }

        #endregion


        #region IMethodSelectorPolicy

        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<object> SelectMethods(ref BuilderContext context)
        {
            return Select(ref context);
        }

        #endregion


        #region Overrides

        protected override MethodInfo[] DeclaredMembers(Type type)
        {
#if NETSTANDARD1_0
            return type.GetMethodsHierarchical()
                       .Where(c => c.IsStatic == false && c.IsPublic)
                       .ToArray();
#else
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                       .ToArray();
#endif
        }

        #endregion
    }
}
