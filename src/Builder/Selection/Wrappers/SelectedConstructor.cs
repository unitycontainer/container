using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Unity.Builder
{
    /// <summary>
    /// Objects of this type encapsulate <see cref="ConstructorInfo"/> and resolve
    /// parameters.
    /// </summary>
    public class SelectedConstructor : SelectedMemberWithParameters<ConstructorInfo>
    {
        /// <summary>
        /// Create a new <see cref="SelectedConstructor"/> instance which
        /// contains the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor to wrap.</param>
        public SelectedConstructor(ConstructorInfo constructor)
            : base(constructor)
        {
        }

        public SelectedConstructor(ConstructorInfo info, object[] parameters)
            : base(info, parameters.Select(p => 
                p is InjectionParameterValue ipv 
                    ? ipv.GetResolver<BuilderContext>(info.DeclaringType) 
                    : p))
        {
        }

        /// <summary>
        /// The constructor this object wraps.
        /// </summary>
        public ConstructorInfo Constructor => MemberInfo;
    }
}
