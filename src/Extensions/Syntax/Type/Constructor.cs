using System.Linq;
using Unity.Extensions.Syntax;
using Unity.Injection;

namespace Unity
{
    public static class TypeRegistrationConstructorExtension
    {
        public static TypeRegistration Constructor(this TypeRegistration registration, params object[] parameters)
        {
            registration.members = registration.members.Append(new InjectionConstructor(parameters));
            return registration;
        }
    }
}
