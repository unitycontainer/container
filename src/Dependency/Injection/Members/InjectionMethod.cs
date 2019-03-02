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
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        #endregion


        #region Overrides

        protected override MethodInfo SelectMember(Type type, InjectionMember _)
        {
            var noData = 0 == (Data?.Length ?? 0);

            foreach (var member in DeclaredMembers(type))
            {
                if (null != Name)
                {
                    if (Name != member.Name) continue;
                    if (noData) return member;
                }

                if (!Data.MatchMemberInfo(member)) continue;

                return member;
            }

            throw new ArgumentException(NoMatchFound);
        }

        public override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            foreach (var member in type.GetDeclaredMethods())
            {
                if (!member.IsFamily && !member.IsPrivate &&
                    !member.IsStatic && member.Name == Name)
                    yield return member;
            }
        }

        public override string ToString()
        {
            return $"Invoke.Method('{Name}', {Data.Signature()})";
        }

#if NETSTANDARD1_0

        public override bool Equals(MethodInfo other)
        {
            if (null == other || other.Name != Name) return false;

            var parameterTypes = other.GetParameters()
                                      .Select(p => p.ParameterType)
                                      .ToArray();

            if (Selection.ContainsGenericParameters)
                return Data.Length == parameterTypes.Length;

            return Selection.GetParameters()
                             .Select(p => p.ParameterType)
                             .SequenceEqual(parameterTypes);
        }

#endif
        #endregion
    }
}
