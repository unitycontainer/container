using System;
using System.Linq;
using Unity.Extensions.Syntax;

namespace Unity
{
    public static class TypeRegistrationRegisterExtension
    {
        public static IUnityContainer Register(this TypeRegistration registration)
            => registration.container.RegisterType(null, registration.type, null, registration.lifetime, registration.members.ToArray());

        public static IUnityContainer RegisterAs(this TypeRegistration registration, string name)
            => registration.container.RegisterType(null, registration.type, name, registration.lifetime, registration.members.ToArray());

        public static IUnityContainer RegisterAs(this TypeRegistration registration, Type type, string name = null)
            => registration.container.RegisterType(type, registration.type, name, registration.lifetime, registration.members.ToArray());

        public static IUnityContainer RegisterAs<TType>(this TypeRegistration registration, string name = null)
            => registration.container.RegisterType(typeof(TType), registration.type, name, registration.lifetime, registration.members.ToArray());
    }
}
