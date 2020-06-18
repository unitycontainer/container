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
    public class InjectionConstructor : MethodBase<ConstructorInfo>
    {
        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="arguments">The values for the constructor's parameters, that will
        /// be used to create objects.</param>
        public InjectionConstructor(params object[] arguments)
            : base(".ctor", arguments)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InjectionConstructor"/> instance which will configure
        /// the container to invoke the given constructor with the given parameters.
        /// </summary>
        /// <param name="info"><see cref="ConstructorInfo"/> of the method to call</param>
        /// <param name="arguments">Arguments to pass to the method</param>
        public InjectionConstructor(ConstructorInfo info, params object[] arguments)
            : base(info, arguments)
        {
        }

        #endregion


        #region Overrides

        public override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => 
            type.GetConstructors(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)
                .Where(SupportedMembersFilter);

        #endregion
    }
}
