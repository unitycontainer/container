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
            : base((string)null, arguments)
        {
        }

        public InjectionConstructor(ConstructorInfo info, params object[] arguments)
            : base((string)null, arguments)
        {
            Selection = info;
        }

        #endregion


        #region Overrides

        protected override ConstructorInfo SelectMember(Type type, InjectionMember _)
        {
            foreach (var member in DeclaredMembers(type))
            {
                if (!Data.MatchMemberInfo(member)) continue;

                return member;
            }

            throw new ArgumentException(NoMatchFound);
        }

        public override IEnumerable<ConstructorInfo> DeclaredMembers(Type type)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(ctor => !ctor.IsFamily && !ctor.IsPrivate && !ctor.IsStatic);
        }

        public override string ToString()
        {
            return $"Invoke.Constructor({Data.Signature()})";
        }

        #endregion
    }
}
