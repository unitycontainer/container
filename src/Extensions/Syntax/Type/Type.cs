using System;
using Unity.Extensions.Syntax;

namespace Unity
{

    public static class TypeRegistrationExtension
    {
        // Type
        public static TypeRegistration Type(this IUnityContainer container, Type type) 
            => new TypeRegistration(container, type);

        public static TypeRegistration Type<TType>(this IUnityContainer container)
            => new TypeRegistration(container, typeof(TType));
    }

}
