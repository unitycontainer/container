using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : MethodBaseMember<ConstructorInfo>
    {
        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="arguments">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public InjectionConstructor(params object[] arguments)
            : base(arguments)
        {
        }

        public InjectionConstructor(ConstructorInfo info, params object[] arguments)
            : base(arguments)
        {
            MemberInfo = info;
        }

        #endregion


        #region Implementation

        public override bool BuildRequired => true;

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
#if NETCOREAPP1_0 || NETSTANDARD1_0
            return type.GetTypeInfo().DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic);
#else
            return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#endif
        }

        #endregion
    }
}
