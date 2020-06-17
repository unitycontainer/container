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
    public class InjectionMethod : MethodBase<MethodInfo>
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

        /// <summary>
        /// Creates a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given method with the given parameters.
        /// </summary>
        /// <param name="info"><see cref="MethodInfo"/> of the method to call</param>
        /// <param name="arguments">Arguments to pass to the method</param>
        public InjectionMethod(MethodInfo info, params object[] arguments)
            : base(info, arguments)
        {
        }

        #endregion


        #region Overrides

        public override IEnumerable<MethodInfo> DeclaredMembers(Type type) => 
            type.GetMethods(BindingFlags)
                .Where(SupportedMembersFilter)
                .Where(member => member.Name == Name);

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return null == Selection
                        ? $"{GetType().Name}: {Name}({Data.Signature()})"
                        : $"{GetType().Name}: {Selection.DeclaringType}.{Name}({Selection.Signature()})";
            }
            else
            {
                return null == Selection
                    ? $"Invoke.Method('{Name}', {Data.Signature()})"
                    : $"Invoke: {Selection.DeclaringType}.{Name}({Selection.Signature()})";
            }
        }

        #endregion
    }
}
