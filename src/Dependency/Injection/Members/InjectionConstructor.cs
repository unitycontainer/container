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


        #region InjectionMember

        public override bool BuildRequired => true;

        #endregion


        #region MethodBaseMember

        protected override IEnumerable<ConstructorInfo> DeclaredMembers(TypeInfo info)
        {
            return info.DeclaredConstructors
                       .Where(c => c.IsStatic == false && c.IsPublic);
        }

        #endregion
    }
}
