using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : InjectionMethodBase<MethodInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given method with the given parameters.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        #endregion


        #region Overrides

        public override IEnumerable<MethodInfo> DeclaredMembers(Type type) => 
            type.GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)
                .Where(SupportedMembersFilter)
                .Where(member => member.Name == Name);

        #endregion
    }
}
